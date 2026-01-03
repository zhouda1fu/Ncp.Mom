using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

/// <summary>
/// 工作中心实体类型配置
/// </summary>
internal class WorkCenterEntityTypeConfiguration : IEntityTypeConfiguration<WorkCenter>
{
    public void Configure(EntityTypeBuilder<WorkCenter> builder)
    {
        builder.ToTable("WorkCenter");

        builder.HasKey(w => w.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();

        builder.Property(w => w.WorkCenterCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.WorkCenterName)
            .IsRequired()
            .HasMaxLength(200);

        // 索引
        builder.HasIndex(w => w.WorkCenterCode).IsUnique();
        builder.HasIndex(w => w.WorkCenterName);
    }
}
