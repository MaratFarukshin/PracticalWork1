namespace PracticalWork.Reports.Contracts.v1.Reports.Response;

/// <summary>
/// Ответ на запрос генерации отчета
/// </summary>
public sealed record GenerateReportResponse(
    Guid Id,
    string Name,
    DateTime GeneratedAt,
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    ReportStatus Status
);

/// <summary>
/// Статус отчета
/// </summary>
public enum ReportStatus
{
    InProgress = 0,
    Generated = 1,
    Error = 2
}

