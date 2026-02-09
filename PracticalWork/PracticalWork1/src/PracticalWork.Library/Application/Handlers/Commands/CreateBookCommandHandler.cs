using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Books;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Events;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды создания книги
/// </summary>
public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly IMessagePublisher _messagePublisher;

    public CreateBookCommandHandler(
        IBookRepository bookRepository,
        IMessagePublisher messagePublisher)
    {
        _bookRepository = bookRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<Guid> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
    {
        command.Book.Status = BookStatus.Available;
        var bookId = await _bookRepository.CreateBook(command.Book);

        // Публикация события BookCreatedEvent
        var book = await _bookRepository.GetBookByIdAsync(bookId);
        var @event = new BookCreatedEvent
        {
            BookId = book.Id,
            Title = book.Title,
            Category = book.Category,
            Authors = book.Authors,
            Description = book.Description,
            Year = book.Year,
            Status = book.Status,
            CreatedAt = book.CreatedAt
        };

        await _messagePublisher.PublishAsync(@event, "book.created", cancellationToken);

        return bookId;
    }
}

