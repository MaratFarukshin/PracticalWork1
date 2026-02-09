using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Library;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Domain.Services;
using PracticalWork.Library.Events;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды выдачи книги
/// </summary>
public sealed class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand, Models.BorrowedBook>
{
    private readonly IBookRepository _bookRepository;
    private readonly IReaderRepository _readerRepository;
    private readonly IBookBorrowRepository _bookBorrowRepository;
    private readonly BookAvailabilityService _bookAvailabilityService;
    private readonly ReaderValidationService _readerValidationService;
    private readonly ICacheService _cacheService;
    private readonly IMessagePublisher _messagePublisher;

    public BorrowBookCommandHandler(
        IBookRepository bookRepository,
        IReaderRepository readerRepository,
        IBookBorrowRepository bookBorrowRepository,
        BookAvailabilityService bookAvailabilityService,
        ReaderValidationService readerValidationService,
        ICacheService cacheService,
        IMessagePublisher messagePublisher)
    {
        _bookRepository = bookRepository;
        _readerRepository = readerRepository;
        _bookBorrowRepository = bookBorrowRepository;
        _bookAvailabilityService = bookAvailabilityService;
        _readerValidationService = readerValidationService;
        _cacheService = cacheService;
        _messagePublisher = messagePublisher;
    }

    public async Task<Models.BorrowedBook> HandleAsync(BorrowBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(command.BookId);
        var reader = await _readerRepository.GetByIdAsync(command.ReaderId);

        if (!_bookAvailabilityService.CanBeBorrowed(book))
        {
            throw new LibraryDomainException($"Книга с идентификатором {command.BookId} недоступна для выдачи.");
        }

        if (!_readerValidationService.IsActive(reader))
        {
            throw new LibraryDomainException($"Карточка читателя с идентификатором {command.ReaderId} неактивна.");
        }

        var borrowDate = DateOnly.FromDateTime(DateTime.Today);
        var dueDate = borrowDate.AddDays(30);

        var borrowId = await _bookBorrowRepository.CreateBorrowAsync(command.BookId, command.ReaderId, borrowDate, dueDate);

        book.Status = BookStatus.Borrow;
        await _bookRepository.UpdateBookAsync(command.BookId, book);

        // Инвалидация кэшей, зависящих от статуса книги и списка книг читателя
        await _cacheService.InvalidateByPatternAsync("books:list:*");
        await _cacheService.InvalidateByPatternAsync("library:books:*");
        await _cacheService.InvalidateAsync($"reader:books:{command.ReaderId}");

        // Публикация события BookBorrowedEvent
        var @event = new BookBorrowedEvent
        {
            BookId = command.BookId,
            ReaderId = command.ReaderId,
            BorrowId = borrowId,
            BorrowDate = borrowDate,
            DueDate = dueDate
        };

        await _messagePublisher.PublishAsync(@event, "book.borrowed", cancellationToken);

        return new Models.BorrowedBook
        {
            BookId = book.Id,
            Title = book.Title,
            Authors = book.Authors,
            BorrowDate = borrowDate,
            DueDate = dueDate
        };
    }
}

