using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class RoutingEntityTypeConfiguration : IEntityTypeConfiguration<Routing>
{
    public void Configure(EntityTypeBuilder<Routing> builder)
    {
        builder.ToTable("routing");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.RoutingNumber).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();

        builder.HasIndex(b => b.RoutingNumber).IsUnique();
        builder.HasIndex(b => b.ProductId);

        builder.HasMany(r => r.Operations)
            .WithOne()
            .HasForeignKey("RoutingId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Operations).AutoInclude();
    }
}

internal class RoutingOperationEntityTypeConfiguration : IEntityTypeConfiguration<RoutingOperation>
{
    public void Configure(EntityTypeBuilder<RoutingOperation> builder)
    {
        builder.ToTable("routing_operation");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(o => o.Sequence).IsRequired();
        builder.Property(o => o.OperationName).HasMaxLength(200).IsRequired();
        builder.Property(o => o.StandardTime).HasPrecision(18, 2).IsRequired();
    }
}

