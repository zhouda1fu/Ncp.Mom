using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.QualityInspectionEndpoints;

/// <summary>
/// 获取质检单详情的API端点
/// </summary>
[Tags("QualityInspections")]
public class GetQualityInspectionEndpoint(QualityInspectionQuery qualityInspectionQuery) 
    : Endpoint<GetQualityInspectionRequest, ResponseData<QualityInspectionDto>>
{
    public override void Configure()
    {
        Get("/api/quality-inspections/{Id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(GetQualityInspectionRequest req, CancellationToken ct)
    {
        var qualityInspection = await qualityInspectionQuery.GetQualityInspectionByIdAsync(req.Id, ct);
        
        if (qualityInspection == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(qualityInspection.AsResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 获取质检单详情的请求模型
/// </summary>
public record GetQualityInspectionRequest
{
    public QualityInspectionId Id { get; set; } = default!;
}

