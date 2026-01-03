using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

/// <summary>
/// 开始维护的请求模型
/// </summary>
/// <param name="Id">设备ID</param>
public record StartMaintenanceRequest(EquipmentId Id);

/// <summary>
/// 开始维护的API端点
/// 该端点用于开始设备的维护
/// </summary>
[Tags("Equipments")]
public class StartMaintenanceEndpoint(IMediator mediator) : Endpoint<StartMaintenanceRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/equipments/{id}/start-maintenance");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行开始维护操作
    /// </summary>
    /// <param name="req">包含设备ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(StartMaintenanceRequest req, CancellationToken ct)
    {
        var cmd = new StartMaintenanceCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

