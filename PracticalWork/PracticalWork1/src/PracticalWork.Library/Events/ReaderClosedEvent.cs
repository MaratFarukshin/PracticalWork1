namespace PracticalWork.Library.Events;

/// <summary>
/// Событие закрытия карточки читателя
/// </summary>
public sealed class ReaderClosedEvent : BaseLibraryEvent
{
    public Guid ReaderId { get; set; }
    public string FullName { get; set; }
    public DateOnly ClosedAt { get; set; }

    public ReaderClosedEvent() : base("ReaderClosed")
    {
    }
}

