using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

public interface IReaderService
{
    /// <summary>
    /// Создание карточки читателя
    /// </summary>
    Task<Guid> CreateReader(Reader reader);

    /// <summary>
    /// Продление срока действия карточки читателя
    /// </summary>
    Task<DateOnly> ExtendReaderAsync(Guid id, DateOnly newExpiryDate);

    /// <summary>
    /// Закрытие карточки читателя
    /// </summary>
    Task CloseReaderAsync(Guid id);

    /// <summary>
    /// Получить список взятых книг читателя
    /// </summary>
    Task<IReadOnlyList<BorrowedBook>> GetBorrowedBooksAsync(Guid id);
}

