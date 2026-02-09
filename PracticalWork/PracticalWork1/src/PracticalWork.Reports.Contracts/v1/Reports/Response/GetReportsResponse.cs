namespace PracticalWork.Reports.Contracts.v1.Reports.Response;

/// <summary>
/// Ответ на запрос получения списка отчетов
/// </summary>
public sealed record GetReportsResponse(
    IReadOnlyList<ReportInfoResponse> Reports
);

/// <summary>
/// Информация об отчете
/// </summary>
public sealed record ReportInfoResponse(
    Guid Id,
    string Name,
    DateTime GeneratedAt,
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    ReportStatus Status
);

