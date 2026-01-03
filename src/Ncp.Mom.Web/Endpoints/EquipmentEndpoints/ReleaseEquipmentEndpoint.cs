using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public record ReleaseEquipmentRequest { public EquipmentId Id { get; set; } = default!; }

public class ReleaseEquipmentEndpoint(IMediator mediator) : Endpoint<ReleaseEquipmentRequest>
{
    public override void Configure()
    {
        Post("/api/equipments/{id}/release");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(ReleaseEquipmentRequest req, CancellationToken ct)
    {
        var cmd = new ReleaseEquipmentCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

