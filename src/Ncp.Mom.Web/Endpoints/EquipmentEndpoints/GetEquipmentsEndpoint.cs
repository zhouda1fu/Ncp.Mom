using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

[Tags("Equipments")]
public class GetEquipmentsEndpoint(EquipmentQuery equipmentQuery) 
    : Endpoint<EquipmentQueryInput, ResponseData<PagedData<EquipmentDto>>>
{
    public override void Configure()
    {
        Get("/api/equipments");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(EquipmentQueryInput req, CancellationToken ct)
    {
        var result = await equipmentQuery.GetEquipmentsAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

