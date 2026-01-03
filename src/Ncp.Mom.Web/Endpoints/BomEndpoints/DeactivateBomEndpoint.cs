using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public record DeactivateBomRequest { public BomId Id { get; set; } = default!; }

public class DeactivateBomEndpoint(IMediator mediator) : Endpoint<DeactivateBomRequest>
{
    public override void Configure()
    {
        Post("/api/boms/{id}/deactivate");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(DeactivateBomRequest req, CancellationToken ct)
    {
        var cmd = new DeactivateBomCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

