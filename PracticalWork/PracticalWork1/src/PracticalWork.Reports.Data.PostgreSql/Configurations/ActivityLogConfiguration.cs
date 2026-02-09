using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Reports.Data.PostgreSql.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Configurations;

internal sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLogEntity>
{
    public void Configure(EntityTypeBuilder<ActivityLogEntity> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExternalBookId);
        builder.Property(e => e.ExternalReaderId);
        builder.Property(e => e.EventType).IsRequired();
        builder.Property(e => e.EventDate).IsRequired();
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(e => e.UpdatedAt);
    }
}

