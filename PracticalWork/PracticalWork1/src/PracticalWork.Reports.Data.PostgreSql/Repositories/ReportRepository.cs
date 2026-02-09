using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Data.PostgreSql.Entities;
using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с отчетами
/// </summary>
public sealed class ReportRepository : IReportRepository
{
    private readonly ReportsDbContext _context;

    public ReportRepository(ReportsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(Report report, CancellationToken cancellationToken = default)
    {
        var entity = new ReportEntity
        {
            Id = report.Id,
            Name = report.Name,
            FilePath = report.FilePath,
            GeneratedAt = report.GeneratedAt,
            PeriodFrom = report.PeriodFrom,
            PeriodTo = report.PeriodTo,
            Status = (int)report.Status,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt
        };

        _context.Reports.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reports.FirstOrDefaultAsync(x => x.Id == report.Id, cancellationToken);
        
        if (entity == null)
        {
            throw new ArgumentException($"Отчет с идентификатором {report.Id} не найден.", nameof(report));
        }

        entity.Name = report.Name;
        entity.FilePath = report.FilePath;
        entity.GeneratedAt = report.GeneratedAt;
        entity.PeriodFrom = report.PeriodFrom;
        entity.PeriodTo = report.PeriodTo;
        entity.Status = (int)report.Status;
        entity.UpdatedAt = report.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Report> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity == null)
        {
            throw new ArgumentException($"Отчет с идентификатором {id} не найден.", nameof(id));
        }

        return MapToReport(entity);
    }

    public async Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Reports
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync(cancellationToken);

        return entities
            .Select(MapToReport)
            .ToList();
    }

    private static Report MapToReport(ReportEntity entity)
    {
        return new Report
        {
            Id = entity.Id,
            Name = entity.Name,
            FilePath = entity.FilePath,
            GeneratedAt = entity.GeneratedAt,
            PeriodFrom = entity.PeriodFrom,
            PeriodTo = entity.PeriodTo,
            Status = (ReportStatus)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

