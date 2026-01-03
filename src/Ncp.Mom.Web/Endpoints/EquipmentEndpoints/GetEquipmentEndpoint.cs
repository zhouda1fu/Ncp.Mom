using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public record GetEquipmentRequest { public EquipmentId Id { get; set; } = default!; }

public class GetEquipmentEndpoint(EquipmentQuery equipmentQuery) 
    : Endpoint<GetEquipmentRequest, ResponseData<EquipmentDto>>
{
    public override void Configure()
    {
        Get("/api/equipments/{Id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(GetEquipmentRequest req, CancellationToken ct)
    {
        var equipment = await equipmentQuery.GetEquipmentByIdAsync(req.Id, ct);
        if (equipment == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(equipment.AsResponseData(), cancellation: ct);
    }
}

