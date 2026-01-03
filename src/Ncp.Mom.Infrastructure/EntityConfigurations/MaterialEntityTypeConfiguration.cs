using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class MaterialEntityTypeConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Material");

        builder.HasKey(m => m.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(m => m.MaterialCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.MaterialName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Specification)
            .HasMaxLength(200);

        builder.Property(m => m.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(m => m.MaterialCode).IsUnique();
        builder.HasIndex(m => m.MaterialName);
    }
}

