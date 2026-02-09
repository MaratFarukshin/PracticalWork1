using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Application.Abstractions;
using PracticalWork.Reports.Application.Commands;
using PracticalWork.Reports.Models;
using System.Text;

namespace PracticalWork.Reports.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды генерации отчета
/// </summary>
public sealed class GenerateReportCommandHandler : ICommandHandler<GenerateReportCommand, Report>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICacheService _cacheService;
    private const string ReportsBucket = "reports";

    public GenerateReportCommandHandler(
        IActivityLogRepository activityLogRepository,
        IReportRepository reportRepository,
        IFileStorageService fileStorageService,
        ICacheService cacheService)
    {
        _activityLogRepository = activityLogRepository;
        _reportRepository = reportRepository;
        _fileStorageService = fileStorageService;
        _cacheService = cacheService;
    }

    public async Task<Report> HandleAsync(GenerateReportCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Валидация параметров отчета
        if (command.PeriodFrom > command.PeriodTo)
        {
            throw new ArgumentException("Начало периода не может быть позже конца периода.");
        }

        // 2. Получение данных об активности
        var activityLogs = await _activityLogRepository.GetActivityLogsAsync(
            fromDate: command.PeriodFrom.ToDateTime(TimeOnly.MinValue),
            toDate: command.PeriodTo.ToDateTime(TimeOnly.MaxValue),
            eventType: command.EventType,
            pageNumber: 1,
            pageSize: int.MaxValue,
            cancellationToken: cancellationToken);

        // 3. Генерация CSV файла
        var csvContent = GenerateCsv(activityLogs.Items, command);

        // 4. Сохранение отчета в MinIO
        var now = DateTime.UtcNow;
        var reportName = $"report_{command.PeriodFrom:yyyyMMdd}_{command.PeriodTo:yyyyMMdd}_{Guid.NewGuid():N}.csv";
        var objectName = $"{now.Year}/{now.Month:D2}/{reportName}";
        
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var filePath = await _fileStorageService.UploadFileAsync(
            ReportsBucket,
            objectName,
            csvStream,
            "text/csv");

        // 5. Сохранение информации об отчете в PostgreSQL
        var report = new Report
        {
            Id = Guid.NewGuid(),
            Name = reportName,
            FilePath = filePath,
            GeneratedAt = now,
            PeriodFrom = command.PeriodFrom,
            PeriodTo = command.PeriodTo,
            Status = ReportStatus.Generated,
            CreatedAt = now
        };

        await _reportRepository.CreateAsync(report, cancellationToken);

        // 6. Инвалидация кэша списка отчетов
        await _cacheService.InvalidateAsync("reports:list");

        return report;
    }

    private static string GenerateCsv(IReadOnlyList<ActivityLog> logs, GenerateReportCommand command)
    {
        var sb = new StringBuilder();
        
        // Заголовок CSV
        sb.AppendLine("EventDate,EventType,BookId,ReaderId,Metadata");
        
        // Данные
        foreach (var log in logs)
        {
            sb.AppendLine($"{log.EventDate:yyyy-MM-dd HH:mm:ss},{log.EventType},{log.ExternalBookId},{log.ExternalReaderId},\"{log.Metadata}\"");
        }
        
        return sb.ToString();
    }
}

