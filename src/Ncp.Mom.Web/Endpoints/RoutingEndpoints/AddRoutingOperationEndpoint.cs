using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

public record AddRoutingOperationRequest
{
    public RoutingId RoutingId { get; set; } = default!;
    public int Sequence { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public WorkCenterId WorkCenterId { get; set; } = default!;
    public decimal StandardTime { get; set; }
}

[Tags("Routings")]
[HttpPost("/api/routings/{routingId}/operations")]
[AllowAnonymous]
public class AddRoutingOperationEndpoint(IMediator mediator)
    : Endpoint<AddRoutingOperationRequest>
{
    public override async Task HandleAsync(
        AddRoutingOperationRequest req,
        CancellationToken ct)
    {
        var cmd = new AddRoutingOperationCommand(
            req.RoutingId,
            req.Sequence,
            req.OperationName,
            req.WorkCenterId,
            req.StandardTime);

        await mediator.Send(cmd, ct);
        await Send.NoContentAsync(ct);
    }
}

