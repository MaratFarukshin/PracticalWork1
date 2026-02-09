namespace PracticalWork.Reports.Contracts.v1.Reports.Response;

/// <summary>
/// Ответ на запрос получения URL для скачивания отчета
/// </summary>
public sealed record GetReportDownloadResponse(string DownloadUrl);

