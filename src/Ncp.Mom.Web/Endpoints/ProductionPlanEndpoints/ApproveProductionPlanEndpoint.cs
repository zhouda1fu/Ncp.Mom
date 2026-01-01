using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Commands.ProductionPlans;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

public record ApproveProductionPlanRequest
{
    public ProductionPlanId Id { get; set; } = default!;
}

[Tags("ProductionPlans")]
[HttpPost("/api/production-plans/{id}/approve")]
[AllowAnonymous]
public class ApproveProductionPlanEndpoint(IMediator mediator)
    : Endpoint<ApproveProductionPlanRequest>
{
    public override async Task HandleAsync(
        ApproveProductionPlanRequest req,
        CancellationToken ct)
    {
        var cmd = new ApproveProductionPlanCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.NoContentAsync(ct);
    }
}

