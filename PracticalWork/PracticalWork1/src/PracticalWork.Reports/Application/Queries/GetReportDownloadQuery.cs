using PracticalWork.Reports.Application.Abstractions;

namespace PracticalWork.Reports.Application.Queries;

/// <summary>
/// Запрос получения URL для скачивания отчета
/// </summary>
public sealed record GetReportDownloadQuery(string ReportName) : IQuery<string>;

