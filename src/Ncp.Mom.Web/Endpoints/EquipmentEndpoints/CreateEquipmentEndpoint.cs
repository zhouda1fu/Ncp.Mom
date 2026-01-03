using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public record CreateEquipmentRequest(
    string EquipmentCode,
    string EquipmentName,
    EquipmentType EquipmentType,
    WorkCenterId? WorkCenterId);

public record CreateEquipmentResponse(EquipmentId Id);

public class CreateEquipmentEndpoint(IMediator mediator) 
    : Endpoint<CreateEquipmentRequest, ResponseData<CreateEquipmentResponse>>
{
    public override void Configure()
    {
        Post("/api/equipments");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(CreateEquipmentRequest req, CancellationToken ct)
    {
        var cmd = new CreateEquipmentCommand(
            req.EquipmentCode,
            req.EquipmentName,
            req.EquipmentType,
            req.WorkCenterId);

        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(new CreateEquipmentResponse(id).AsResponseData(), cancellation: ct);
    }
}

