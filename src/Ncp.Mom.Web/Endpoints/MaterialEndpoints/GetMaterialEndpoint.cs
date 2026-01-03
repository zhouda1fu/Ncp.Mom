using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

[Tags("Materials")]
public record GetMaterialRequest { public MaterialId Id { get; set; } = default!; }

public class GetMaterialEndpoint(MaterialQuery materialQuery) 
    : Endpoint<GetMaterialRequest, ResponseData<MaterialDto>>
{
    public override void Configure()
    {
        Get("/api/materials/{Id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(GetMaterialRequest req, CancellationToken ct)
    {
        var material = await materialQuery.GetMaterialByIdAsync(req.Id, ct);
        if (material == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(material.AsResponseData(), cancellation: ct);
    }
}

