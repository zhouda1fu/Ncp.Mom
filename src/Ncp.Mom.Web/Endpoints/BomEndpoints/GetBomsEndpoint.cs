using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public class GetBomsEndpoint(BomQuery bomQuery) 
    : Endpoint<BomQueryInput, ResponseData<PagedData<BomDto>>>
{
    public override void Configure()
    {
        Get("/api/boms");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(BomQueryInput req, CancellationToken ct)
    {
        var result = await bomQuery.GetBomsAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

