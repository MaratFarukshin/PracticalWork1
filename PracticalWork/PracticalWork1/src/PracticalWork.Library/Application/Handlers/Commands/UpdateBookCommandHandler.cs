using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Books;
using PracticalWork.Library.Domain.Exceptions;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды обновления книги
/// </summary>
public sealed class UpdateBookCommandHandler : ICommandHandler<UpdateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;

    public UpdateBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task HandleAsync(UpdateBookCommand command, CancellationToken cancellationToken = default)
    {
        var existingBook = await _bookRepository.GetBookByIdAsync(command.Id);

        if (existingBook.IsArchived)
        {
            throw new BookDomainException($"Книга с идентификатором {command.Id} находится в архиве и не может быть отредактирована.");
        }

        existingBook.Title = command.Book.Title;
        existingBook.Description = command.Book.Description;
        existingBook.Year = command.Book.Year;
        existingBook.Authors = command.Book.Authors;

        await _bookRepository.UpdateBookAsync(command.Id, existingBook);

        await _cacheService.InvalidateAsync($"book:{command.Id}");
        await _cacheService.InvalidateByPatternAsync("books:*");
    }
}

