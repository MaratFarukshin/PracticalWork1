using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models;

/// <summary>
/// Книга с информацией о выдаче
/// </summary>
public sealed class BookWithBorrowInfo
{
    /// <summary>Идентификатор книги</summary>
    public Guid Id { get; set; }

    /// <summary>Название книги</summary>
    public string Title { get; set; }

    /// <summary>Авторы</summary>
    public IReadOnlyList<string> Authors { get; set; }

    /// <summary>Краткое описание книги</summary>
    public string Description { get; set; }

    /// <summary>Год издания</summary>
    public int Year { get; set; }

    /// <summary>Категория</summary>
    public BookCategory Category { get; set; }

    /// <summary>Статус</summary>
    public BookStatus Status { get; set; }

    /// <summary>Путь к изображению обложки</summary>
    public string CoverImagePath { get; set; }

    /// <summary>В архиве</summary>
    public bool IsArchived { get; set; }

    /// <summary>Идентификатор читателя, которому выдана книга (null, если не выдана)</summary>
    public Guid? ReaderId { get; set; }

    /// <summary>Дата выдачи книги (null, если не выдана)</summary>
    public DateOnly? BorrowDate { get; set; }

    /// <summary>Срок возврата книги (null, если не выдана)</summary>
    public DateOnly? DueDate { get; set; }
}

