using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Application.Queries;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения списка отчетов
/// </summary>
public sealed class GetReportsQueryHandler : IQueryHandler<GetReportsQuery, IReadOnlyList<ReportInfo>>
{
    private readonly ICacheService _cacheService;
    private readonly IReportRepository _reportRepository;
    private const string CacheKey = "reports:list";

    public GetReportsQueryHandler(
        ICacheService cacheService,
        IReportRepository reportRepository)
    {
        _cacheService = cacheService;
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<ReportInfo>> HandleAsync(GetReportsQuery query, CancellationToken cancellationToken = default)
    {
        // 1. Проверка кэша Redis
        var cached = await _cacheService.GetAsync<IReadOnlyList<ReportInfo>>(CacheKey);
        if (cached != null)
        {
            return cached;
        }

        // 2. Запрос к репозиторию для получения списка отчетов
        var reports = await _reportRepository.GetAllAsync(cancellationToken);

        var result = reports
            .OrderByDescending(r => r.GeneratedAt)
            .Select(r => new ReportInfo(
                r.Id,
                r.Name,
                r.GeneratedAt,
                r.PeriodFrom,
                r.PeriodTo,
                r.Status
            ))
            .ToList();

        // 3. Сохранение в кэш на 24 часа
        await _cacheService.SetAsync(CacheKey, result, TimeSpan.FromHours(24));

        return result;
    }
}

