using PracticalWork.Library.Models;

namespace PracticalWork.Library.Domain.Services;

/// <summary>
/// Доменный сервис для проверки доступности книги
/// </summary>
public sealed class BookAvailabilityService
{
    /// <summary>
    /// Проверить, может ли книга быть выдана
    /// </summary>
    public bool CanBeBorrowed(Book book)
    {
        return !book.IsArchived && book.Status == Enums.BookStatus.Available;
    }

    /// <summary>
    /// Проверить, может ли книга быть заархивирована
    /// </summary>
    public bool CanBeArchived(Book book)
    {
        return book.Status != Enums.BookStatus.Borrow;
    }
}

