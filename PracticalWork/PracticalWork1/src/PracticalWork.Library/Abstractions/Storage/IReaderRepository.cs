using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IReaderRepository
{
    /// <summary>
    /// Создать карточку читателя
    /// </summary>
    Task<Guid> CreateReader(Reader reader);

    /// <summary>
    /// Проверить существование читателя по номеру телефона
    /// </summary>
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Получить читателя по идентификатору
    /// </summary>
    Task<Reader> GetByIdAsync(Guid id);

    /// <summary>
    /// Обновить дату окончания действия карточки
    /// </summary>
    Task UpdateExpiryDateAsync(Guid id, DateOnly newExpiryDate);

    /// <summary>
    /// Получить идентификаторы книг, которые сейчас выданы читателю
    /// </summary>
    Task<IReadOnlyList<Guid>> GetActiveBorrowedBookIdsAsync(Guid readerId);

    /// <summary>
    /// Закрыть карточку читателя (сделать неактивной и установить дату окончания)
    /// </summary>
    Task CloseReaderAsync(Guid id, DateOnly closeDate);

    /// <summary>
    /// Получить активные выдачи книг для читателя
    /// </summary>
    Task<IReadOnlyList<BorrowedBook>> GetActiveBorrowedBooksAsync(Guid readerId);
}

