using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Data.PostgreSql.Entities;

namespace PracticalWork.Reports.Data.PostgreSql;

/// <summary>
/// Контекст EF Core для сервиса Отчеты
/// </summary>
public class ReportsDbContext : DbContext
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ReportsDbContext).Assembly);
    }

    #region Set UpdateDate on SaveChanges

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdateDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetUpdateDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetUpdateDates()
    {
        var updateDate = DateTime.UtcNow;

        var updatedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEntries)
        {
            if (entry.Entity is Abstractions.Storage.IEntity entity)
                entity.UpdatedAt = updateDate;
        }
    }

    #endregion

    internal DbSet<ActivityLogEntity> ActivityLogs { get; set; }
    internal DbSet<ReportEntity> Reports { get; set; }
}

