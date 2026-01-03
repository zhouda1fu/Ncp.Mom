using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public record AssignEquipmentRequest
{
    public EquipmentId Id { get; set; } = default!;
    public WorkOrderId WorkOrderId { get; set; } = default!;
}

public class AssignEquipmentEndpoint(IMediator mediator) : Endpoint<AssignEquipmentRequest>
{
    public override void Configure()
    {
        Post("/api/equipments/{id}/assign");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(AssignEquipmentRequest req, CancellationToken ct)
    {
        var cmd = new AssignEquipmentCommand(req.Id, req.WorkOrderId);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

