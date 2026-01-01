using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

namespace Ncp.Mom.Web.Application.Queries.WorkOrders;

public class WorkOrderQueryInput : PageRequest
{
    public ProductionPlanId? ProductionPlanId { get; set; }
    public WorkOrderStatus? Status { get; set; }
}

public record WorkOrderDto(
    WorkOrderId Id,
    string WorkOrderNumber,
    ProductionPlanId ProductionPlanId,
    ProductId ProductId,
    int Quantity,
    int CompletedQuantity,
    RoutingId RoutingId,
    WorkOrderStatus Status,
    DateTime? StartTime,
    DateTime? EndTime,
    DateTimeOffset UpdateTime);

