using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Commands.ProductionPlans;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

public record StartProductionPlanRequest
{
    public ProductionPlanId Id { get; set; } = default!;
}

[Tags("ProductionPlans")]
[HttpPost("/api/production-plans/{id}/start")]
[AllowAnonymous]
public class StartProductionPlanEndpoint(IMediator mediator)
    : Endpoint<StartProductionPlanRequest>
{
    public override async Task HandleAsync(
        StartProductionPlanRequest req,
        CancellationToken ct)
    {
        var cmd = new StartProductionPlanCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.NoContentAsync(ct);
    }
}

