using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

[Tags("Materials")]
public class GetMaterialsEndpoint(MaterialQuery materialQuery) 
    : Endpoint<MaterialQueryInput, ResponseData<PagedData<MaterialDto>>>
{
    public override void Configure()
    {
        Get("/api/materials");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(MaterialQueryInput req, CancellationToken ct)
    {
        var result = await materialQuery.GetMaterialsAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

