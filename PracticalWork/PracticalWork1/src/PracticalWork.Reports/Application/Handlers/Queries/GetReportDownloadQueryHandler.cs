using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Application.Queries;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения URL для скачивания отчета
/// </summary>
public sealed class GetReportDownloadQueryHandler : IQueryHandler<GetReportDownloadQuery, string>
{
    private readonly IFileStorageService _fileStorageService;
    private const string ReportsBucket = "reports";

    public GetReportDownloadQueryHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<string> HandleAsync(GetReportDownloadQuery query, CancellationToken cancellationToken = default)
    {
        // 1. Поиск отчета по имени в MinIO
        // Формат имени: report_YYYYMMDD_YYYYMMDD_GUID.csv
        // Ищем файл в бакете reports
        var files = await _fileStorageService.ListFilesAsync(ReportsBucket);
        var reportFile = files.FirstOrDefault(f => f.Contains(query.ReportName) && f.EndsWith(".csv"));
        
        if (reportFile == null)
        {
            throw new FileNotFoundException($"Отчет '{query.ReportName}' не найден.");
        }

        // 2. Извлекаем objectName из полного пути (убираем bucket name)
        var objectName = reportFile.StartsWith($"{ReportsBucket}/") 
            ? reportFile.Substring($"{ReportsBucket}/".Length) 
            : reportFile;

        // 3. Генерация signed URL для скачивания (TTL 1 час)
        var signedUrl = await _fileStorageService.GenerateSignedUrlAsync(
            ReportsBucket,
            objectName,
            TimeSpan.FromHours(1));

        return signedUrl;
    }
}

