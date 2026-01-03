using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.QualityInspectionEndpoints;

/// <summary>
/// 获取质检单列表的API端点
/// </summary>
[Tags("QualityInspections")]
public class GetQualityInspectionsEndpoint(QualityInspectionQuery qualityInspectionQuery) 
    : Endpoint<QualityInspectionQueryInput, ResponseData<PagedData<QualityInspectionDto>>>
{
    public override void Configure()
    {
        Get("/api/quality-inspections");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(QualityInspectionQueryInput req, CancellationToken ct)
    {
        var result = await qualityInspectionQuery.GetQualityInspectionsAsync(req, ct);
        await Send.OkAsync(result.AsResponseData(), cancellation: ct);
    }
}

