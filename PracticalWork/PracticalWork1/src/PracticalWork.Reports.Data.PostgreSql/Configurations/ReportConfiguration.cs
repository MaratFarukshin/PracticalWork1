using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Reports.Data.PostgreSql.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Configurations;

internal sealed class ReportConfiguration : IEntityTypeConfiguration<ReportEntity>
{
    public void Configure(EntityTypeBuilder<ReportEntity> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.GeneratedAt).IsRequired();
        builder.Property(e => e.PeriodFrom).IsRequired();
        builder.Property(e => e.PeriodTo).IsRequired();
        builder.Property(e => e.Status).IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(e => e.UpdatedAt);
    }
}

