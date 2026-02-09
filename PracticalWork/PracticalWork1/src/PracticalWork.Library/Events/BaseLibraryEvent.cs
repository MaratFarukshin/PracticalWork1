namespace PracticalWork.Library.Events;

/// <summary>
/// Базовое сообщение для событий библиотеки
/// </summary>
public abstract class BaseLibraryEvent
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Дата отправки события
    /// </summary>
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Тип события
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Источник события
    /// </summary>
    public string Source { get; set; } = "library service";

    protected BaseLibraryEvent(string eventType)
    {
        EventType = eventType;
    }
}

