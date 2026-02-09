using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Queries.Readers;
using PracticalWork.Library.Domain.Exceptions;

namespace PracticalWork.Library.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения списка книг читателя
/// </summary>
public sealed class GetReaderBooksQueryHandler : IQueryHandler<GetReaderBooksQuery, IReadOnlyList<Models.BorrowedBook>>
{
    private readonly IReaderRepository _readerRepository;
    private readonly ICacheService _cacheService;

    public GetReaderBooksQueryHandler(IReaderRepository readerRepository, ICacheService cacheService)
    {
        _readerRepository = readerRepository;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<Models.BorrowedBook>> HandleAsync(GetReaderBooksQuery query, CancellationToken cancellationToken = default)
    {
        // Проверка существования карточки
        await _readerRepository.GetByIdAsync(query.ReaderId);

        var cacheKey = $"reader:books:{query.ReaderId}";
        var cached = await _cacheService.GetAsync<IReadOnlyList<Models.BorrowedBook>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var books = await _readerRepository.GetActiveBorrowedBooksAsync(query.ReaderId);

        await _cacheService.SetAsync(cacheKey, books, TimeSpan.FromMinutes(15));

        return books;
    }
}

