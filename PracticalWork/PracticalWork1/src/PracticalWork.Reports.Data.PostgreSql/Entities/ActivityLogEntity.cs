using PracticalWork.Reports.Abstractions.Storage;

namespace PracticalWork.Reports.Data.PostgreSql.Entities;

/// <summary>
/// Сущность лога активности
/// </summary>
public sealed class ActivityLogEntity : EntityBase
{
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
}

