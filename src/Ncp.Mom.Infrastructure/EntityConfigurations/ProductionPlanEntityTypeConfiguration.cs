using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class ProductionPlanEntityTypeConfiguration : IEntityTypeConfiguration<ProductionPlan>
{
    public void Configure(EntityTypeBuilder<ProductionPlan> builder)
    {
        builder.ToTable("production_plan");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.PlanNumber).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Quantity).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.PlannedStartDate).IsRequired();
        builder.Property(b => b.PlannedEndDate).IsRequired();

        builder.HasIndex(b => b.PlanNumber).IsUnique();
    }
}

