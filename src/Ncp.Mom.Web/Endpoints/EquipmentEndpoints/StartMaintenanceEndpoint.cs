using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public record StartMaintenanceRequest { public EquipmentId Id { get; set; } = default!; }

public class StartMaintenanceEndpoint(IMediator mediator) : Endpoint<StartMaintenanceRequest>
{
    public override void Configure()
    {
        Post("/api/equipments/{id}/start-maintenance");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(StartMaintenanceRequest req, CancellationToken ct)
    {
        var cmd = new StartMaintenanceCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

