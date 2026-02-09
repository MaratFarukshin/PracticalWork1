using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Queries.Books;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения списка книг
/// </summary>
public sealed class GetBooksQueryHandler : IQueryHandler<GetBooksQuery, Models.PagedResult<Models.Book>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;

    public GetBooksQueryHandler(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<Models.PagedResult<Models.Book>> HandleAsync(GetBooksQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = GenerateCacheKey(query.Status, query.Category, query.Author, query.PageNumber, query.PageSize);
        var cachedResult = await _cacheService.GetAsync<Models.PagedResult<Models.Book>>(cacheKey);
        
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var result = await _bookRepository.GetBooksAsync(query.Status, query.Category, query.Author, query.PageNumber, query.PageSize);
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }

    private static string GenerateCacheKey(BookStatus? status, BookCategory? category, string author, int pageNumber, int pageSize)
    {
        // Формируем детальное строковое представление фильтров и пагинации
        var raw = $"status={status?.ToString() ?? "any"};" +
                  $"category={category?.ToString() ?? "any"};" +
                  $"author={author?.ToLowerInvariant() ?? "any"};" +
                  $"page={pageNumber};size={pageSize}";

        // Преобразуем в "хеш" (достаточно стабильно и просто, крипто‑безопасность не требуется)
        var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
        var hash = Convert.ToBase64String(bytes);

        return $"books:list:{hash}";
    }
}

