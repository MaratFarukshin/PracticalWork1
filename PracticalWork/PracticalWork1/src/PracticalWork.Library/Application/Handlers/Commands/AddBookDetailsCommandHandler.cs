using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Books;
using PracticalWork.Library.Domain.Exceptions;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды добавления деталей книги
/// </summary>
public sealed class AddBookDetailsCommandHandler : ICommandHandler<AddBookDetailsCommand, string>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private const string BookCoversBucket = "book-covers";
    private const long MaxCoverImageSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public AddBookDetailsCommandHandler(
        IBookRepository bookRepository,
        ICacheService cacheService,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
    }

    public async Task<string> HandleAsync(AddBookDetailsCommand command, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetBookByIdAsync(command.Id);

        var validationResult = _fileValidationService.ValidateCoverImage(
            command.CoverImageStream, command.FileName, command.ContentType, MaxCoverImageSizeInBytes);
        
        if (!validationResult.IsValid)
        {
            throw new BookDomainException(validationResult.ErrorMessage);
        }

        // Путь согласно требованиям: book-covers/{year}/{month}/{bookId}.{extension}
        var now = DateTime.UtcNow;
        var extension = Path.GetExtension(command.FileName);
        var objectName = $"{now.Year}/{now.Month:D2}/{command.Id}{extension}";

        var coverImagePath = await _fileStorageService.UploadFileAsync(
            BookCoversBucket, objectName, command.CoverImageStream, command.ContentType);

        book.UpdateDetails(command.Description, coverImagePath);
        await _bookRepository.UpdateBookAsync(command.Id, book);

        await _cacheService.InvalidateAsync($"book:details:{command.Id}");
        await _cacheService.InvalidateByPatternAsync("books:list:*");
        await _cacheService.InvalidateByPatternAsync("library:books:*");

        return coverImagePath;
    }
}

