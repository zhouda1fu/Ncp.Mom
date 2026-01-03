using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

/// <summary>
/// 释放设备的请求模型
/// </summary>
/// <param name="Id">设备ID</param>
public record ReleaseEquipmentRequest(EquipmentId Id);

/// <summary>
/// 释放设备的API端点
/// 该端点用于释放设备（从工单中释放）
/// </summary>
[Tags("Equipments")]
public class ReleaseEquipmentEndpoint(IMediator mediator) : Endpoint<ReleaseEquipmentRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/equipments/{id}/release");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行释放设备操作
    /// </summary>
    /// <param name="req">包含设备ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(ReleaseEquipmentRequest req, CancellationToken ct)
    {
        var cmd = new ReleaseEquipmentCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

