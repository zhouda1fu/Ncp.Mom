using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Queries.ProductionPlans;

namespace Ncp.Mom.Web.Application.Queries;

public class ProductionPlanQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<ProductionPlan> ProductionPlanSet { get; } = applicationDbContext.ProductionPlans;

    public async Task<bool> DoesProductionPlanExist(string planNumber, CancellationToken cancellationToken)
    {
        return await ProductionPlanSet.AsNoTracking()
            .AnyAsync(p => p.PlanNumber == planNumber, cancellationToken);
    }

    public async Task<PagedData<ProductionPlanDto>> GetProductionPlansAsync(
        ProductionPlanQueryInput query,
        CancellationToken cancellationToken)
    {
        var queryable = ProductionPlanSet.AsNoTracking();

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(p => p.Status == query.Status.Value);
        }

        return await queryable
            .OrderByDescending(p => p.Id)
            .Select(p => new ProductionPlanDto(
                p.Id,
                p.PlanNumber,
                p.ProductId,
                p.Quantity,
                p.Status,
                p.PlannedStartDate,
                p.PlannedEndDate,
                p.UpdateTime.Value))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

