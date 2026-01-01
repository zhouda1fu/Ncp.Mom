using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class WorkOrderEntityTypeConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("work_order");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.WorkOrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Quantity).IsRequired();
        builder.Property(b => b.CompletedQuantity).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.StartTime).IsRequired(false);
        builder.Property(b => b.EndTime).IsRequired(false);

        builder.HasIndex(b => b.WorkOrderNumber).IsUnique();
        builder.HasIndex(b => b.ProductionPlanId);
    }
}

