namespace PracticalWork.Reports.Models;

/// <summary>
/// Отчет
/// </summary>
public sealed class Report
{
    /// <summary>Идентификатор отчета</summary>
    public Guid Id { get; set; }

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
    public ReportStatus Status { get; set; }

    /// <summary>Дата создания</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата обновления</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Статус отчета
/// </summary>
public enum ReportStatus
{
    /// <summary>В процессе</summary>
    InProgress = 0,

    /// <summary>Сгенерирован</summary>
    Generated = 1,

    /// <summary>Ошибка</summary>
    Error = 2
}

/// <summary>
/// Тип события
/// </summary>
public enum EventType
{
    BookCreated = 1,
    BookArchived = 2,
    ReaderCreated = 3,
    ReaderClosed = 4,
    BookBorrowed = 5,
    BookReturned = 6
}

