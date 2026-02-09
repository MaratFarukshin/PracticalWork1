using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Library;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Events;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды возврата книги
/// </summary>
public sealed class ReturnBookCommandHandler : ICommandHandler<ReturnBookCommand, Models.ReturnedBook>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookBorrowRepository _bookBorrowRepository;
    private readonly ICacheService _cacheService;
    private readonly IMessagePublisher _messagePublisher;

    public ReturnBookCommandHandler(
        IBookRepository bookRepository,
        IBookBorrowRepository bookBorrowRepository,
        ICacheService cacheService,
        IMessagePublisher messagePublisher)
    {
        _bookRepository = bookRepository;
        _bookBorrowRepository = bookBorrowRepository;
        _cacheService = cacheService;
        _messagePublisher = messagePublisher;
    }

    public async Task<Models.ReturnedBook> HandleAsync(ReturnBookCommand command, CancellationToken cancellationToken = default)
    {
        var borrowId = await _bookBorrowRepository.FindActiveBorrowAsync(command.BookId, command.ReaderId);
        
        if (!borrowId.HasValue)
        {
            throw new LibraryDomainException($"Активная запись о выдаче книги {command.BookId} читателю {command.ReaderId} не найдена.");
        }

        var book = await _bookRepository.GetBookByIdAsync(command.BookId);
        
        if (book.Status != BookStatus.Borrow)
        {
            throw new LibraryDomainException($"Книга с идентификатором {command.BookId} не находится в статусе 'Выдана'.");
        }

        var returnDate = DateOnly.FromDateTime(DateTime.Today);
        var dueDate = await _bookBorrowRepository.GetDueDateAsync(borrowId.Value);
        
        if (!dueDate.HasValue)
        {
            throw new LibraryDomainException($"Не удалось получить информацию о сроке возврата для записи {borrowId.Value}.");
        }

        var wasOverdue = returnDate > dueDate.Value;
        var status = wasOverdue ? BookIssueStatus.Overdue : BookIssueStatus.Returned;

        await _bookBorrowRepository.UpdateBorrowAsync(borrowId.Value, returnDate, status);

        book.Status = BookStatus.Available;
        await _bookRepository.UpdateBookAsync(command.BookId, book);

        // Инвалидация кэшей, зависящих от статуса книги и списка книг читателя
        await _cacheService.InvalidateByPatternAsync("books:list:*");
        await _cacheService.InvalidateByPatternAsync("library:books:*");
        await _cacheService.InvalidateAsync($"reader:books:{command.ReaderId}");

        // Публикация события BookReturnedEvent
        var @event = new BookReturnedEvent
        {
            BookId = command.BookId,
            ReaderId = command.ReaderId,
            BorrowId = borrowId.Value,
            ReturnDate = returnDate,
            WasOverdue = wasOverdue
        };

        await _messagePublisher.PublishAsync(@event, "book.returned", cancellationToken);

        return new Models.ReturnedBook
        {
            BookId = command.BookId,
            ReaderId = command.ReaderId,
            ReturnDate = returnDate,
            WasOverdue = wasOverdue
        };
    }
}

