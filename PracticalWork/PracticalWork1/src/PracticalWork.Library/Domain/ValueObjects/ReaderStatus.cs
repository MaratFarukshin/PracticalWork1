namespace PracticalWork.Library.Domain.ValueObjects;

/// <summary>
/// Статус читателя (Value Object)
/// </summary>
public sealed class ReaderStatus
{
    /// <summary>Активен ли читатель</summary>
    public bool IsActive { get; }

    /// <summary>Дата окончания действия карточки</summary>
    public DateOnly ExpiryDate { get; }

    public ReaderStatus(bool isActive, DateOnly expiryDate)
    {
        IsActive = isActive;
        ExpiryDate = expiryDate;
    }

    /// <summary>
    /// Проверить, действительна ли карточка
    /// </summary>
    public bool IsValid(DateOnly currentDate)
    {
        return IsActive && ExpiryDate >= currentDate;
    }

    /// <summary>
    /// Создать активный статус
    /// </summary>
    public static ReaderStatus Active(DateOnly expiryDate) => new(true, expiryDate);

    /// <summary>
    /// Создать неактивный статус
    /// </summary>
    public static ReaderStatus Inactive(DateOnly expiryDate) => new(false, expiryDate);
}

