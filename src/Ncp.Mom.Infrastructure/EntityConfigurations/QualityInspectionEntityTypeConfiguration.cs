using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

/// <summary>
/// 质检单实体类型配置
/// </summary>
internal class QualityInspectionEntityTypeConfiguration : IEntityTypeConfiguration<QualityInspection>
{
    public void Configure(EntityTypeBuilder<QualityInspection> builder)
    {
        builder.ToTable("QualityInspection");

        builder.HasKey(q => q.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(q => q.InspectionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.WorkOrderId)
            .IsRequired();

        builder.Property(q => q.SampleQuantity)
            .IsRequired();

        builder.Property(q => q.QualifiedQuantity)
            .IsRequired();

        builder.Property(q => q.UnqualifiedQuantity)
            .IsRequired();

        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(q => q.Remark)
            .HasMaxLength(500);

        // 索引
        builder.HasIndex(q => q.InspectionNumber).IsUnique();
        builder.HasIndex(q => q.WorkOrderId);
        builder.HasIndex(q => q.Status);
    }
}

