using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Queries;

/// <summary>
/// Запрос получения списка отчетов
/// </summary>
public sealed record GetReportsQuery() : IQuery<IReadOnlyList<ReportInfo>>;

/// <summary>
/// Информация об отчете для списка
/// </summary>
public sealed record ReportInfo(
    Guid Id,
    string Name,
    DateTime GeneratedAt,
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    ReportStatus Status
);

