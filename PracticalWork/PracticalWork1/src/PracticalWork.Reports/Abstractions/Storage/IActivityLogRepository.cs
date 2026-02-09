using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Abstractions.Storage;

/// <summary>
/// Репозиторий для работы с логами активности
/// </summary>
public interface IActivityLogRepository
{
    /// <summary>
    /// Создать лог активности
    /// </summary>
    Task<Guid> CreateAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить логи активности с фильтрацией и пагинацией
    /// </summary>
    Task<PagedResult<ActivityLog>> GetActivityLogsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EventType? eventType = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}

