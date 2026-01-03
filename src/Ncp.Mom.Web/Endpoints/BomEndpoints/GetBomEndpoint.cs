using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public record GetBomRequest { public BomId Id { get; set; } = default!; }

public class GetBomEndpoint(BomQuery bomQuery) 
    : Endpoint<GetBomRequest, ResponseData<BomDto>>
{
    public override void Configure()
    {
        Get("/api/boms/{Id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(GetBomRequest req, CancellationToken ct)
    {
        var bom = await bomQuery.GetBomByIdAsync(req.Id, ct);
        if (bom == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(bom.AsResponseData(), cancellation: ct);
    }
}

