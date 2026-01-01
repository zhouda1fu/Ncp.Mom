using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.Application.Queries.WorkOrders;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

[Tags("WorkOrders")]
[HttpGet("/api/work-orders")]
[AllowAnonymous]
public class GetWorkOrdersEndpoint(WorkOrderQuery workOrderQuery)
    : Endpoint<WorkOrderQueryInput, ResponseData<PagedData<WorkOrderDto>>>
{
    public override async Task HandleAsync(
        WorkOrderQueryInput req,
        CancellationToken ct)
    {
        var result = await workOrderQuery.GetWorkOrdersAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

