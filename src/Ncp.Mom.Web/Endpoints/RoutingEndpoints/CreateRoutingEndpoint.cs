using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

public record CreateRoutingRequest(
    string RoutingNumber,
    string Name,
    ProductId ProductId);

[Tags("Routings")]
[HttpPost("/api/routings")]
[AllowAnonymous]
public class CreateRoutingEndpoint(IMediator mediator)
    : Endpoint<CreateRoutingRequest, ResponseData<RoutingId>>
{
    public override async Task HandleAsync(
        CreateRoutingRequest req,
        CancellationToken ct)
    {
        var cmd = new CreateRoutingCommand(
            req.RoutingNumber,
            req.Name,
            req.ProductId);

        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(id.AsResponseData(), cancellation: ct);
    }
}

