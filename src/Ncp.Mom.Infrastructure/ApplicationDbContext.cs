using Ncp.Mom.Domain.AggregatesModel.OrderAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.DistributedTransactions.CAP.Persistence;
using Ncp.Mom.Domain.AggregatesModel.DeliverAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Infrastructure;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
    : AppDbContextBase(options, mediator)
    , IMySqlCapDataStorage
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }



    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ConfigureStronglyTypedIdValueConverter(configurationBuilder);
        base.ConfigureConventions(configurationBuilder);
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<DeliverRecord> DeliverRecords => Set<DeliverRecord>();
    public DbSet<ProductionPlan> ProductionPlans => Set<ProductionPlan>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Routing> Routings => Set<Routing>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WorkCenter> WorkCenters => Set<WorkCenter>();
}
