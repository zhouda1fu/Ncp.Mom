using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.WorkOrders;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

public record StartWorkOrderRequest
{
    public WorkOrderId Id { get; set; } = default!;
}

[Tags("WorkOrders")]
[HttpPost("/api/work-orders/{id}/start")]
[AllowAnonymous]
public class StartWorkOrderEndpoint(IMediator mediator)
    : Endpoint<StartWorkOrderRequest>
{
    public override async Task HandleAsync(
        StartWorkOrderRequest req,
        CancellationToken ct)
    {
        var cmd = new StartWorkOrderCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.NoContentAsync(ct);
    }
}

