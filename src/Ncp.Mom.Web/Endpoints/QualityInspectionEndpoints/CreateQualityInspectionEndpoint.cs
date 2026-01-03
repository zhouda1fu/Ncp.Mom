using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.QualityInspections;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.QualityInspectionEndpoints;

/// <summary>
/// 创建质检单的请求模型
/// </summary>
public record CreateQualityInspectionRequest(
    string InspectionNumber,
    WorkOrderId WorkOrderId,
    int SampleQuantity);

/// <summary>
/// 创建质检单的响应模型
/// </summary>
public record CreateQualityInspectionResponse(QualityInspectionId Id);

/// <summary>
/// 创建质检单的API端点
/// </summary>
[Tags("QualityInspections")]
public class CreateQualityInspectionEndpoint(IMediator mediator) 
    : Endpoint<CreateQualityInspectionRequest, ResponseData<CreateQualityInspectionResponse>>
{
    public override void Configure()
    {
        Post("/api/quality-inspections");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(CreateQualityInspectionRequest req, CancellationToken ct)
    {
        var cmd = new CreateQualityInspectionCommand(
            req.InspectionNumber,
            req.WorkOrderId,
            req.SampleQuantity);

        var id = await mediator.Send(cmd, ct);

        var response = new CreateQualityInspectionResponse(id);
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

