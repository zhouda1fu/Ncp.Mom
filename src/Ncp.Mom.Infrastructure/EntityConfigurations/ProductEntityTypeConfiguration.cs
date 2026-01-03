using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

/// <summary>
/// 产品实体类型配置
/// </summary>
internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(p => p.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(p => p.ProductCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        // 索引
        builder.HasIndex(p => p.ProductCode).IsUnique();
        builder.HasIndex(p => p.ProductName);
    }
}
