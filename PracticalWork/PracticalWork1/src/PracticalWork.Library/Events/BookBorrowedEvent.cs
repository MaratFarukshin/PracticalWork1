namespace PracticalWork.Library.Events;

/// <summary>
/// Событие выдачи книги
/// </summary>
public sealed class BookBorrowedEvent : BaseLibraryEvent
{
    public Guid BookId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid BorrowId { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }

    public BookBorrowedEvent() : base("BookBorrowed")
    {
    }
}

