using Ncp.Mom.Domain.AggregatesModel.DeliverAggregate;

namespace Ncp.Mom.Infrastructure.EntityConfigurations
{
    internal class DeliverRecordConfiguration : IEntityTypeConfiguration<DeliverRecord>
    {
        public void Configure(EntityTypeBuilder<DeliverRecord> builder)
        {
            builder.ToTable("deliverrecord");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        }
    }

}
