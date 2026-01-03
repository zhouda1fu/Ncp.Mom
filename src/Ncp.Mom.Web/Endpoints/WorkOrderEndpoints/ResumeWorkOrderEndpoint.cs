using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.WorkOrders;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.WorkOrderEndpoints;

/// <summary>
/// 恢复工单的请求模型
/// </summary>
/// <param name="Id">工单ID</param>
public record ResumeWorkOrderRequest(WorkOrderId Id);

/// <summary>
/// 恢复工单的API端点
/// 该端点用于恢复已暂停的工单
/// </summary>
[Tags("WorkOrders")]
public class ResumeWorkOrderEndpoint(IMediator mediator) : Endpoint<ResumeWorkOrderRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP POST方法，用于恢复工单
        Post("/api/work-orders/{id}/resume");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工单编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.WorkOrderEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行恢复操作
    /// </summary>
    /// <param name="req">包含工单ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(ResumeWorkOrderRequest req, CancellationToken ct)
    {
        // 创建恢复工单命令对象
        var cmd = new ResumeWorkOrderCommand(req.Id);

        // 通过中介者发送命令，执行实际的恢复业务逻辑
        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

