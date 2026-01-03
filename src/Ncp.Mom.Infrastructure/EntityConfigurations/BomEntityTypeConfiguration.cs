using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class BomEntityTypeConfiguration : IEntityTypeConfiguration<Bom>
{
    public void Configure(EntityTypeBuilder<Bom> builder)
    {
        builder.ToTable("Bom");

        builder.HasKey(b => b.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(b => b.BomNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Version)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired();

        builder.OwnsMany(b => b.Items, itemBuilder =>
        {
            itemBuilder.ToTable("BomItem");
            itemBuilder.WithOwner().HasForeignKey("BomId");
            itemBuilder.HasKey(i => i.Id);
            itemBuilder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
            itemBuilder.Property(i => i.Quantity).IsRequired().HasPrecision(18, 4);
            itemBuilder.Property(i => i.Unit).IsRequired().HasMaxLength(20);
        });

        builder.HasIndex(b => b.BomNumber).IsUnique();
        builder.HasIndex(b => b.ProductId);
        builder.HasIndex(b => b.IsActive);
    }
}

