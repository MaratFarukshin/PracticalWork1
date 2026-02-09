using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для операций библиотеки (выдача/возврат)
/// </summary>
public interface ILibraryService
{
    /// <summary>
    /// Выдача книги читателю
    /// </summary>
    Task<BorrowedBook> BorrowBookAsync(Guid bookId, Guid readerId);
    
    /// <summary>
    /// Получить список книг библиотеки с информацией о выдаче
    /// </summary>
    Task<PagedResult<BookWithBorrowInfo>> GetLibraryBooksAsync(
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10);
    
    /// <summary>
    /// Возврат книги читателем
    /// </summary>
    Task<ReturnedBook> ReturnBookAsync(Guid bookId, Guid readerId);
    
    /// <summary>
    /// Получить детальную информацию о книге по ID или названию
    /// </summary>
    Task<BookDetails> GetLibraryBookDetailsAsync(string idOrTitle);
}