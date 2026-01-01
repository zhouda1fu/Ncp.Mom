using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IProductionPlanRepository : IRepository<ProductionPlan, ProductionPlanId>
{
}

public class ProductionPlanRepository(ApplicationDbContext context)
    : RepositoryBase<ProductionPlan, ProductionPlanId, ApplicationDbContext>(context),
      IProductionPlanRepository
{
}

