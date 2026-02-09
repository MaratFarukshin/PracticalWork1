using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Books;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Domain.Services;
using PracticalWork.Library.Events;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды архивирования книги
/// </summary>
public sealed class ArchiveBookCommandHandler : ICommandHandler<ArchiveBookCommand, Models.Book>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    private readonly ArchiveCheckService _archiveCheckService;
    private readonly IMessagePublisher _messagePublisher;

    public ArchiveBookCommandHandler(
        IBookRepository bookRepository,
        ICacheService cacheService,
        ArchiveCheckService archiveCheckService,
        IMessagePublisher messagePublisher)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _archiveCheckService = archiveCheckService;
        _messagePublisher = messagePublisher;
    }

    public async Task<Models.Book> HandleAsync(ArchiveBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(command.Id);

        if (!_archiveCheckService.CanBeArchived(book))
        {
            throw new BookDomainException($"Книга с идентификатором {command.Id} не может быть заархивирована, так как она выдана читателю.");
        }

        book.Archive();
        await _bookRepository.UpdateBookAsync(command.Id, book);

        await _cacheService.InvalidateAsync($"book:{command.Id}");
        await _cacheService.InvalidateByPatternAsync("books:*");
        await _cacheService.InvalidateByPatternAsync("library:books:*");

        // Публикация события BookArchivedEvent
        var @event = new BookArchivedEvent
        {
            BookId = book.Id,
            Title = book.Title,
            ArchivedAt = DateTime.UtcNow
        };

        await _messagePublisher.PublishAsync(@event, "book.archived", cancellationToken);

        return book;
    }
}

