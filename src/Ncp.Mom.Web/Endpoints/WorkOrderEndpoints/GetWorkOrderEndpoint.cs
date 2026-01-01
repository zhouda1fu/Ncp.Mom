using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Queries.WorkOrders;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

public record GetWorkOrderRequest
{
    public WorkOrderId Id { get; set; } = default!;
}

[Tags("WorkOrders")]
[HttpGet("/api/work-orders/{id}")]
[AllowAnonymous]
public class GetWorkOrderEndpoint(IMediator mediator)
    : Endpoint<GetWorkOrderRequest, ResponseData<WorkOrderDto>>
{
    public override async Task HandleAsync(
        GetWorkOrderRequest req,
        CancellationToken ct)
    {
        var query = new GetWorkOrderQuery(req.Id);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

