using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Web.Application.Commands.QualityInspections;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.QualityInspectionEndpoints;

/// <summary>
/// 执行质检的请求模型
/// </summary>
public record InspectQualityRequest
{
    public QualityInspectionId Id { get; set; } = default!;
    public int QualifiedQuantity { get; set; }
    public int UnqualifiedQuantity { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 执行质检的API端点
/// 该端点用于执行质检操作
/// </summary>
[Tags("QualityInspections")]
public class InspectQualityEndpoint(IMediator mediator) 
    : Endpoint<InspectQualityRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/quality-inspections/{id}/inspect");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行质检操作
    /// </summary>
    /// <param name="req">包含质检信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(InspectQualityRequest req, CancellationToken ct)
    {
        var cmd = new InspectQualityCommand(
            req.Id,
            req.QualifiedQuantity,
            req.UnqualifiedQuantity,
            req.Remark);

        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

