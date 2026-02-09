using PracticalWork.Reports.Abstractions.Storage;

namespace PracticalWork.Reports.Data.PostgreSql.Entities;

/// <summary>
/// Сущность отчета
/// </summary>
public sealed class ReportEntity : EntityBase
{
    /// <summary>Название отчета</summary>
    public required string Name { get; set; }

    /// <summary>Путь к файлу в MinIO</summary>
    public required string FilePath { get; set; }

    /// <summary>Дата генерации</summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>Начало периода отчета</summary>
    public DateOnly PeriodFrom { get; set; }

    /// <summary>Конец периода отчета</summary>
    public DateOnly PeriodTo { get; set; }

    /// <summary>Статус отчета</summary>
    public int Status { get; set; }
}

