using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Events;

/// <summary>
/// Событие создания книги
/// </summary>
public sealed class BookCreatedEvent : BaseLibraryEvent
{
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public BookCategory Category { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public BookStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public BookCreatedEvent() : base("BookCreated")
    {
    }
}

