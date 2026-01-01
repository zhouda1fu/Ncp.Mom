using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Web.Application.Queries.Routings;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

public record GetRoutingRequest
{
    public RoutingId Id { get; set; } = default!;
}

[Tags("Routings")]
[HttpGet("/api/routings/{id}")]
[AllowAnonymous]
public class GetRoutingEndpoint(IMediator mediator)
    : Endpoint<GetRoutingRequest, ResponseData<RoutingDto>>
{
    public override async Task HandleAsync(
        GetRoutingRequest req,
        CancellationToken ct)
    {
        var query = new GetRoutingQuery(req.Id);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

