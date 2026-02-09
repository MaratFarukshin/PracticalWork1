namespace PracticalWork.Reports.Contracts.v1.Reports.Request;

/// <summary>
/// Запрос на генерацию отчета
/// </summary>
public sealed record GenerateReportRequest(
    DateOnly PeriodFrom,
    DateOnly PeriodTo,
    EventType? EventType = null
);

/// <summary>
/// Тип события для фильтрации
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

