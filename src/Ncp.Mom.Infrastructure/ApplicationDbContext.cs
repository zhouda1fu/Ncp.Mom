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
using Ncp.Mom.Domain.AggregatesModel.UserAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoleAggregate;
using Ncp.Mom.Domain.AggregatesModel.OrganizationUnitAggregate;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;

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
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<UserOrganizationUnit> UserOrganizationUnits => Set<UserOrganizationUnit>();
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Bom> Boms => Set<Bom>();
    public DbSet<Material> Materials => Set<Material>();
}
