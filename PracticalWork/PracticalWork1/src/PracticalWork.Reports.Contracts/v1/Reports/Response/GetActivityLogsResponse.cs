namespace PracticalWork.Reports.Contracts.v1.Reports.Response;

/// <summary>
/// Ответ на запрос получения логов активности
/// </summary>
public sealed record GetActivityLogsResponse(
    IReadOnlyList<ActivityLogItemResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Элемент лога активности
/// </summary>
public sealed record ActivityLogItemResponse(
    Guid Id,
    Guid? ExternalBookId,
    Guid? ExternalReaderId,
    int EventType,
    DateTime EventDate,
    string Metadata
);

