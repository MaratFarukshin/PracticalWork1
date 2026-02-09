using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models;

/// <summary>
/// Детальная информация о книге
/// </summary>
public sealed class BookDetails
{
    /// <summary>Идентификатор книги</summary>
    public Guid Id { get; set; }

    /// <summary>Название книги</summary>
    public string Title { get; set; }

    /// <summary>Категория</summary>
    public BookCategory Category { get; set; }

    /// <summary>Авторы</summary>
    public IReadOnlyList<string> Authors { get; set; }

    /// <summary>Краткое описание книги</summary>
    public string Description { get; set; }

    /// <summary>Год издания</summary>
    public int Year { get; set; }

    /// <summary>URL для доступа к обложке</summary>
    public string CoverImageUrl { get; set; }

    /// <summary>Статус</summary>
    public BookStatus Status { get; set; }

    /// <summary>В архиве</summary>
    public bool IsArchived { get; set; }
}

