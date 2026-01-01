using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("product");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.ProductCode).HasMaxLength(50).IsRequired();
        builder.Property(b => b.ProductName).HasMaxLength(200).IsRequired();

        builder.HasIndex(b => b.ProductCode).IsUnique();
    }
}

