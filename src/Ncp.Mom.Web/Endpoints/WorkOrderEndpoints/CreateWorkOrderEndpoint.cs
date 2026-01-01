using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Web.Application.Commands.WorkOrders;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

public record CreateWorkOrderRequest(
    string WorkOrderNumber,
    ProductionPlanId ProductionPlanId,
    ProductId ProductId,
    int Quantity,
    RoutingId RoutingId);

[Tags("WorkOrders")]
[HttpPost("/api/work-orders")]
[AllowAnonymous]
public class CreateWorkOrderEndpoint(IMediator mediator)
    : Endpoint<CreateWorkOrderRequest, ResponseData<WorkOrderId>>
{
    public override async Task HandleAsync(
        CreateWorkOrderRequest req,
        CancellationToken ct)
    {
        var cmd = new CreateWorkOrderCommand(
            req.WorkOrderNumber,
            req.ProductionPlanId,
            req.ProductId,
            req.Quantity,
            req.RoutingId);

        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(id.AsResponseData(), cancellation: ct);
    }
}

