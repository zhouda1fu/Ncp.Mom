using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.EquipmentEndpoints;

/// <summary>
/// 分配设备的请求模型
/// </summary>
public record AssignEquipmentRequest
{
    public EquipmentId Id { get; set; } = default!;
    public WorkOrderId WorkOrderId { get; set; } = default!;
}

/// <summary>
/// 分配设备的API端点
/// 该端点用于将设备分配给工单
/// </summary>
[Tags("Equipments")]
public class AssignEquipmentEndpoint(IMediator mediator) : Endpoint<AssignEquipmentRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/equipments/{id}/assign");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行分配设备操作
    /// </summary>
    /// <param name="req">包含设备ID和工单ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(AssignEquipmentRequest req, CancellationToken ct)
    {
        var cmd = new AssignEquipmentCommand(req.Id, req.WorkOrderId);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

