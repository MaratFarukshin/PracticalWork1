using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Репозиторий для работы с выдачами книг
/// </summary>
public interface IBookBorrowRepository
{
    /// <summary>
    /// Создать запись о выдаче книги
    /// </summary>
    Task<Guid> CreateBorrowAsync(Guid bookId, Guid readerId, DateOnly borrowDate, DateOnly dueDate);
    
    /// <summary>
    /// Найти активную запись о выдаче книги читателю
    /// </summary>
    /// <returns>Идентификатор записи о выдаче или null, если не найдена</returns>
    Task<Guid?> FindActiveBorrowAsync(Guid bookId, Guid readerId);
    
    /// <summary>
    /// Получить дату возврата (DueDate) для активной выдачи
    /// </summary>
    Task<DateOnly?> GetDueDateAsync(Guid borrowId);
    
    /// <summary>
    /// Обновить запись о выдаче (дата возврата и статус)
    /// </summary>
    Task UpdateBorrowAsync(Guid borrowId, DateOnly returnDate, BookIssueStatus status);
}