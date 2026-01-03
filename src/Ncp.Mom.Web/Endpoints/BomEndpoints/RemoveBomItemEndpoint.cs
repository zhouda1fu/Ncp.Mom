using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public record RemoveBomItemRequest
{
    public BomId Id { get; set; } = default!;
    public BomItemId ItemId { get; set; } = default!;
}

public class RemoveBomItemEndpoint(IMediator mediator) : Endpoint<RemoveBomItemRequest>
{
    public override void Configure()
    {
        Delete("/api/boms/{id}/items/{itemId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(RemoveBomItemRequest req, CancellationToken ct)
    {
        var cmd = new RemoveBomItemCommand(req.Id, req.ItemId);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

