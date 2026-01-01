using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Commands.ProductionPlans;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

public record CreateProductionPlanRequest(
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate);

[Tags("ProductionPlans")]
[HttpPost("/api/production-plans")]
[AllowAnonymous]
public class CreateProductionPlanEndpoint(IMediator mediator)
    : Endpoint<CreateProductionPlanRequest, ResponseData<ProductionPlanId>>
{
    public override async Task HandleAsync(
        CreateProductionPlanRequest req,
        CancellationToken ct)
    {
        var cmd = new CreateProductionPlanCommand(
            req.PlanNumber,
            req.ProductId,
            req.Quantity,
            req.PlannedStartDate,
            req.PlannedEndDate);

        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(id.AsResponseData(), cancellation: ct);
    }
}

