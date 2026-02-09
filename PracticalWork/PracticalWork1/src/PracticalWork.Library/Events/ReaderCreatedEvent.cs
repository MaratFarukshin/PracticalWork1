namespace PracticalWork.Library.Events;

/// <summary>
/// Событие создания карточки читателя
/// </summary>
public sealed class ReaderCreatedEvent : BaseLibraryEvent
{
    public Guid ReaderId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ReaderCreatedEvent() : base("ReaderCreated")
    {
    }
}

