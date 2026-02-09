namespace PracticalWork.Library.Events;

/// <summary>
/// Событие архивирования книги
/// </summary>
public sealed class BookArchivedEvent : BaseLibraryEvent
{
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public DateTime ArchivedAt { get; set; }

    public BookArchivedEvent() : base("BookArchived")
    {
    }
}

