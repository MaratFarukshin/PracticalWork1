namespace PracticalWork.Library.Models;

/// <summary>
/// Возвращенная книга
/// </summary>
public sealed class ReturnedBook
{
    /// <summary>Идентификатор книги</summary>
    public Guid BookId { get; set; }

    /// <summary>Идентификатор читателя</summary>
    public Guid ReaderId { get; set; }

    /// <summary>Дата возврата книги</summary>
    public DateOnly ReturnDate { get; set; }

    /// <summary>Была ли книга возвращена с просрочкой</summary>
    public bool WasOverdue { get; set; }
}

