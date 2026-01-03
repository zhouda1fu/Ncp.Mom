using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment");

        builder.HasKey(e => e.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(e => e.EquipmentCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.EquipmentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EquipmentType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(e => e.EquipmentCode).IsUnique();
        builder.HasIndex(e => e.WorkCenterId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CurrentWorkOrderId);
    }
}

