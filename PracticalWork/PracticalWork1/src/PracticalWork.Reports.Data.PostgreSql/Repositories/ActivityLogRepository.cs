using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Data.PostgreSql.Entities;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с логами активности
/// </summary>
public sealed class ActivityLogRepository : IActivityLogRepository
{
    private readonly ReportsDbContext _context;

    public ActivityLogRepository(ReportsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(ActivityLog activityLog, CancellationToken cancellationToken = default)
    {
        var entity = new ActivityLogEntity
        {
            Id = activityLog.Id,
            ExternalBookId = activityLog.ExternalBookId,
            ExternalReaderId = activityLog.ExternalReaderId,
            EventType = activityLog.EventType,
            EventDate = activityLog.EventDate,
            Metadata = activityLog.Metadata,
            CreatedAt = activityLog.CreatedAt,
            UpdatedAt = activityLog.UpdatedAt
        };

        _context.ActivityLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<PagedResult<ActivityLog>> GetActivityLogsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        EventType? eventType = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ActivityLogs.AsQueryable();

        // Фильтрация по дате (только если указаны)
        if (fromDate.HasValue)
        {
            query = query.Where(x => x.EventDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.EventDate <= toDate.Value);
        }

        // Фильтрация по типу события (только если указан)
        if (eventType.HasValue)
        {
            query = query.Where(x => x.EventType == (int)eventType.Value);
        }

        // Сортировка по дате события (сначала свежие)
        query = query.OrderByDescending(x => x.EventDate);

        // Подсчет общего количества
        var totalCount = await query.CountAsync(cancellationToken);

        // Применение пагинации
        var skip = (pageNumber - 1) * pageSize;
        var entities = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var logs = entities.Select(MapToActivityLog).ToList();

        return new PagedResult<ActivityLog>
        {
            Items = logs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static ActivityLog MapToActivityLog(ActivityLogEntity entity)
    {
        return new ActivityLog
        {
            Id = entity.Id,
            ExternalBookId = entity.ExternalBookId,
            ExternalReaderId = entity.ExternalReaderId,
            EventType = entity.EventType,
            EventDate = entity.EventDate,
            Metadata = entity.Metadata,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

