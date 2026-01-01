using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Queries.ProductionPlans;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

public record GetProductionPlanRequest
{
    public ProductionPlanId Id { get; set; } = default!;
}

[Tags("ProductionPlans")]
[HttpGet("/api/production-plans/{id}")]
[AllowAnonymous]
public class GetProductionPlanEndpoint(IMediator mediator)
    : Endpoint<GetProductionPlanRequest, ResponseData<ProductionPlanDto>>
{
    public override async Task HandleAsync(
        GetProductionPlanRequest req,
        CancellationToken ct)
    {
        var query = new GetProductionPlanQuery(req.Id);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

