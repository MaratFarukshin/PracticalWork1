namespace PracticalWork.Library.Events;

/// <summary>
/// Событие возврата книги
/// </summary>
public sealed class BookReturnedEvent : BaseLibraryEvent
{
    public Guid BookId { get; set; }
    public Guid ReaderId { get; set; }
    public Guid BorrowId { get; set; }
    public DateOnly ReturnDate { get; set; }
    public bool WasOverdue { get; set; }

    public BookReturnedEvent() : base("BookReturned")
    {
    }
}

