using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Queries.Library;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения списка книг библиотеки
/// </summary>
public sealed class GetLibraryBooksQueryHandler : IQueryHandler<GetLibraryBooksQuery, Models.PagedResult<Models.BookWithBorrowInfo>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;

    public GetLibraryBooksQueryHandler(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<Models.PagedResult<Models.BookWithBorrowInfo>> HandleAsync(GetLibraryBooksQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = GenerateLibraryBooksCacheKey(query.Category, query.Author, query.PageNumber, query.PageSize);
        var cachedResult = await _cacheService.GetAsync<Models.PagedResult<Models.BookWithBorrowInfo>>(cacheKey);
        
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var result = await _bookRepository.GetLibraryBooksAsync(query.Category, query.Author, query.PageNumber, query.PageSize);
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    private static string GenerateLibraryBooksCacheKey(BookCategory? category, string author, int pageNumber, int pageSize)
    {
        var raw = $"category={category?.ToString() ?? "any"};" +
                  $"author={author?.ToLowerInvariant() ?? "any"};" +
                  $"page={pageNumber};size={pageSize}";

        var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
        var hash = Convert.ToBase64String(bytes);

        return $"library:books:{hash}";
    }
}

