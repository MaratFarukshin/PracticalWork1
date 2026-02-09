namespace PracticalWork.Reports.Models;

/// <summary>
/// Лог активности системы
/// </summary>
public sealed class ActivityLog
{
    /// <summary>Идентификатор лога</summary>
    public Guid Id { get; set; }

    /// <summary>Внешний ключ на Book (опционально)</summary>
    public Guid? ExternalBookId { get; set; }

    /// <summary>Внешний ключ на Reader (опционально)</summary>
    public Guid? ExternalReaderId { get; set; }

    /// <summary>Тип события</summary>
    public int EventType { get; set; }

    /// <summary>Дата события</summary>
    public DateTime EventDate { get; set; }

    /// <summary>Дополнительная информация (JSONB)</summary>
    public required string Metadata { get; set; }

    /// <summary>Дата создания</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата обновления</summary>
    public DateTime? UpdatedAt { get; set; }
}

