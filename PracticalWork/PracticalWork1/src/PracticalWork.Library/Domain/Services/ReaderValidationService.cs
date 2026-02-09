using PracticalWork.Library.Models;

namespace PracticalWork.Library.Domain.Services;

/// <summary>
/// Доменный сервис для валидации читателя
/// </summary>
public sealed class ReaderValidationService
{
    /// <summary>
    /// Проверить, активен ли читатель
    /// </summary>
    public bool IsActive(Reader reader)
    {
        return reader.IsActive;
    }

    /// <summary>
    /// Проверить, может ли карточка быть продлена
    /// </summary>
    public bool CanBeExtended(Reader reader, DateOnly newExpiryDate)
    {
        if (!reader.IsActive)
            return false;

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (newExpiryDate <= today)
            return false;

        if (newExpiryDate <= reader.ExpiryDate)
            return false;

        return true;
    }

    /// <summary>
    /// Проверить, может ли карточка быть закрыта
    /// </summary>
    public bool CanBeClosed(Reader reader, IReadOnlyList<Guid> activeBookIds)
    {
        return activeBookIds.Count == 0;
    }
}

