using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

/// <summary>
/// 停用BOM的请求模型
/// </summary>
/// <param name="Id">BOM ID</param>
public record DeactivateBomRequest(BomId Id);

/// <summary>
/// 停用BOM的API端点
/// 该端点用于停用BOM
/// </summary>
[Tags("Boms")]
public class DeactivateBomEndpoint(IMediator mediator) : Endpoint<DeactivateBomRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/boms/{id}/deactivate");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行停用BOM操作
    /// </summary>
    /// <param name="req">包含BOM ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(DeactivateBomRequest req, CancellationToken ct)
    {
        var cmd = new DeactivateBomCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

