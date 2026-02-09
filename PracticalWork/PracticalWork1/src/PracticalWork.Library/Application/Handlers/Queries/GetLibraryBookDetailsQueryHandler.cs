using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Queries.Library;
using PracticalWork.Library.Domain.Exceptions;

namespace PracticalWork.Library.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения деталей книги библиотеки
/// </summary>
public sealed class GetLibraryBookDetailsQueryHandler : IQueryHandler<GetLibraryBookDetailsQuery, Models.BookDetails>
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cacheService;

    public GetLibraryBookDetailsQueryHandler(
        IBookRepository bookRepository,
        IFileStorageService fileStorageService,
        ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
    }

    public async Task<Models.BookDetails> HandleAsync(GetLibraryBookDetailsQuery query, CancellationToken cancellationToken = default)
    {
        // Сначала находим книгу (по id или названию), чтобы получить ее Id для ключа кэша
        var book = await _bookRepository.FindBookByIdOrTitleAsync(query.IdOrTitle);

        var cacheKey = $"book:details:{book.Id}";
        var cached = await _cacheService.GetAsync<Models.BookDetails>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var coverImageUrl = !string.IsNullOrWhiteSpace(book.CoverImagePath)
            ? _fileStorageService.GetFileUrl(book.CoverImagePath)
            : null;

        var details = new Models.BookDetails
        {
            Id = book.Id,
            Title = book.Title,
            Category = book.Category,
            Authors = book.Authors,
            Description = book.Description,
            Year = book.Year,
            CoverImageUrl = coverImageUrl,
            Status = book.Status,
            IsArchived = book.IsArchived
        };

        await _cacheService.SetAsync(cacheKey, details, TimeSpan.FromMinutes(30));

        return details;
    }
}

