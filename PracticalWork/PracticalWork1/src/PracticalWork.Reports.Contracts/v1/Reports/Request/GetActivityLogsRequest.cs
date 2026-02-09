namespace PracticalWork.Reports.Contracts.v1.Reports.Request;

/// <summary>
/// Запрос получения логов активности
/// </summary>
public sealed record GetActivityLogsRequest(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    EventType? EventType = null,
    int PageNumber = 1,
    int PageSize = 20
);

