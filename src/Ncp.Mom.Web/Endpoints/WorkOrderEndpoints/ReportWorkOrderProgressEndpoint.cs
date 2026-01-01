using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.WorkOrders;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

public record ReportWorkOrderProgressRequest
{
    public WorkOrderId Id { get; set; } = default!;
    public int Quantity { get; set; }
}

[Tags("WorkOrders")]
[HttpPost("/api/work-orders/{id}/report-progress")]
[AllowAnonymous]
public class ReportWorkOrderProgressEndpoint(IMediator mediator)
    : Endpoint<ReportWorkOrderProgressRequest>
{
    public override async Task HandleAsync(
        ReportWorkOrderProgressRequest req,
        CancellationToken ct)
    {
        var cmd = new ReportWorkOrderProgressCommand(req.Id, req.Quantity);
        await mediator.Send(cmd, ct);
        await Send.NoContentAsync(ct);
    }
}

