using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace Ncp.Mom.Web.Application.Queries.ProductionPlans;

public record GetProductionPlanQuery(ProductionPlanId Id) : IQuery<ProductionPlanDto>;

public class GetProductionPlanQueryHandler(ApplicationDbContext applicationDbContext)
    : IQueryHandler<GetProductionPlanQuery, ProductionPlanDto>
{
    public async Task<ProductionPlanDto> Handle(
        GetProductionPlanQuery request,
        CancellationToken cancellationToken)
    {
        var result = await applicationDbContext.Set<ProductionPlan>()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductionPlanDto(
                p.Id,
                p.PlanNumber,
                p.ProductId,
                p.Quantity,
                p.Status,
                p.PlannedStartDate,
                p.PlannedEndDate,
                p.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
        return result ?? throw new KnownException($"未找到生产计划，Id = {request.Id}");
    }
}

