using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

public interface IBookService
{
    /// <summary>
    /// Создание книги
    /// </summary>
    Task<Guid> CreateBook(Book book);
    
    /// <summary>
    /// Обновление книги
    /// </summary>
    Task UpdateBookAsync(Guid id, Book book);
    
    /// <summary>
    /// Перевод книги в архив
    /// </summary>
    Task<Book> ArchiveBookAsync(Guid id);
    
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
    /// Добавить детали книги (описание и обложку)
    /// </summary>
    /// <returns>Путь к загруженной обложке</returns>
    Task<string> AddBookDetailsAsync(Guid id, string description, Stream coverImageStream, string fileName, string contentType);
}