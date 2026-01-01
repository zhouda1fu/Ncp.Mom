using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IWorkOrderRepository : IRepository<WorkOrder, WorkOrderId>
{
    Task<List<WorkOrder>> GetByProductionPlanIdAsync(ProductionPlanId productionPlanId, CancellationToken cancellationToken = default);
}

public class WorkOrderRepository(ApplicationDbContext context)
    : RepositoryBase<WorkOrder, WorkOrderId, ApplicationDbContext>(context),
      IWorkOrderRepository
{
    public async Task<List<WorkOrder>> GetByProductionPlanIdAsync(
        ProductionPlanId productionPlanId,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<WorkOrder>()
            .Where(w => w.ProductionPlanId == productionPlanId)
            .ToListAsync(cancellationToken);
    }
}

