using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Application.Queries;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения логов активности
/// </summary>
public sealed class GetActivityLogsQueryHandler : IQueryHandler<GetActivityLogsQuery, PagedResult<ActivityLog>>
{
    private readonly IActivityLogRepository _activityLogRepository;

    public GetActivityLogsQueryHandler(IActivityLogRepository activityLogRepository)
    {
        _activityLogRepository = activityLogRepository;
    }

    public async Task<PagedResult<ActivityLog>> HandleAsync(GetActivityLogsQuery query, CancellationToken cancellationToken = default)
    {
        return await _activityLogRepository.GetActivityLogsAsync(
            fromDate: query.FromDate,
            toDate: query.ToDate,
            eventType: query.EventType,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken);
    }
}

