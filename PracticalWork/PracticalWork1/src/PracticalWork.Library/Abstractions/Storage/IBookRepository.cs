using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookRepository
{
    Task<Guid> CreateBook(Book book);
    
    /// <summary>
    /// Получить книгу по идентификатору
    /// </summary>
    Task<Book> GetBookByIdAsync(Guid id);
    
    /// <summary>
    /// Обновить книгу
    /// </summary>
    Task UpdateBookAsync(Guid id, Book book);
    
    /// <summary>
    /// Получить список книг с фильтрацией и пагинацией
    /// </summary>
    Task<PagedResult<Book>> GetBooksAsync(
        BookStatus? status = null,
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10);
    
    /// <summary>
    /// Получить список не архивных книг с информацией о выдаче, фильтрацией и пагинацией
    /// </summary>
    Task<PagedResult<BookWithBorrowInfo>> GetLibraryBooksAsync(
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10);
    
    /// <summary>
    /// Найти книгу по идентификатору или названию
    /// </summary>
    Task<Book> FindBookByIdOrTitleAsync(string idOrTitle);
}