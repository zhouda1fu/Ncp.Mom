using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class WorkCenterEntityTypeConfiguration : IEntityTypeConfiguration<WorkCenter>
{
    public void Configure(EntityTypeBuilder<WorkCenter> builder)
    {
        builder.ToTable("work_center");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.WorkCenterCode).HasMaxLength(50).IsRequired();
        builder.Property(b => b.WorkCenterName).HasMaxLength(200).IsRequired();

        builder.HasIndex(b => b.WorkCenterCode).IsUnique();
    }
}

