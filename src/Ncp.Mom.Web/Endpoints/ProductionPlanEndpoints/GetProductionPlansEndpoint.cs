using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.Application.Queries.ProductionPlans;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

[Tags("ProductionPlans")]
[HttpGet("/api/production-plans")]
[AllowAnonymous]
public class GetProductionPlansEndpoint(ProductionPlanQuery productionPlanQuery)
    : Endpoint<ProductionPlanQueryInput, ResponseData<PagedData<ProductionPlanDto>>>
{
    public override async Task HandleAsync(
        ProductionPlanQueryInput req,
        CancellationToken ct)
    {
        var result = await productionPlanQuery.GetProductionPlansAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

