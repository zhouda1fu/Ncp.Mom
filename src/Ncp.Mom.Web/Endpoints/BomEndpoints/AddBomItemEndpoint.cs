using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public record AddBomItemRequest
{
    public BomId Id { get; set; } = default!;
    public MaterialId MaterialId { get; set; } = default!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class AddBomItemEndpoint(IMediator mediator) : Endpoint<AddBomItemRequest>
{
    public override void Configure()
    {
        Post("/api/boms/{id}/items");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(AddBomItemRequest req, CancellationToken ct)
    {
        var cmd = new AddBomItemCommand(req.Id, req.MaterialId, req.Quantity, req.Unit);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

