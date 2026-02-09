namespace PracticalWork.Library.Models;

/// <summary>
/// Взятая читателем книга
/// </summary>
public sealed class BorrowedBook
{
    /// <summary>Идентификатор книги</summary>
    public Guid BookId { get; set; }

    /// <summary>Название книги</summary>
    public string Title { get; set; }

    /// <summary>Авторы</summary>
    public IReadOnlyList<string> Authors { get; set; }

    /// <summary>Дата выдачи книги</summary>
    public DateOnly BorrowDate { get; set; }

    /// <summary>Срок возврата книги</summary>
    public DateOnly DueDate { get; set; }
}