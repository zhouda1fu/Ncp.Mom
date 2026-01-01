using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;

namespace Ncp.Mom.Web.Application.Queries.ProductionPlans;

public class ProductionPlanQueryInput : PageRequest
{
    public ProductionPlanStatus? Status { get; set; }
}

public record ProductionPlanDto(
    ProductionPlanId Id,
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    ProductionPlanStatus Status,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate,
    DateTimeOffset UpdateTime);

