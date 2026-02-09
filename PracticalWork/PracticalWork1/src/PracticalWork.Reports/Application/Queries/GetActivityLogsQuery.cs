using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Queries;

/// <summary>
/// Запрос получения логов активности
/// </summary>
public sealed record GetActivityLogsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    EventType? EventType = null,
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResult<ActivityLog>>;

