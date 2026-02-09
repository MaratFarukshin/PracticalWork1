namespace PracticalWork.Library.Models;

/// <summary>
/// Карточка читателя
/// </summary>
public sealed class Reader
{
    /// <summary>Идентификатор читателя</summary>
    public Guid Id { get; set; }

    /// <summary>ФИО</summary>
    /// <remarks>Запись идет через пробел</remarks>
    public string FullName { get; set; }

    /// <summary>Номер телефона</summary>
    public string PhoneNumber { get; set; }

    /// <summary>Дата окончания действия карточки</summary>
    public DateOnly ExpiryDate { get; set; }

    /// <summary>Активность карточки</summary>
    public bool IsActive { get; set; }

    /// <summary>Дата создания</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата обновления</summary>
    public DateTime? UpdatedAt { get; set; }
}

