using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

/// <summary>
/// 删除工艺路线工序的请求模型
/// </summary>
/// <param name="RoutingId">工艺路线ID</param>
/// <param name="Sequence">工序序号</param>
public record RemoveRoutingOperationRequest(RoutingId RoutingId, int Sequence);

/// <summary>
/// 删除工艺路线工序的API端点
/// 该端点用于从工艺路线中删除指定的工序
/// </summary>
[Tags("Routings")]
public class RemoveRoutingOperationEndpoint(IMediator mediator) : Endpoint<RemoveRoutingOperationRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP DELETE方法，用于删除工序
        Delete("/api/routings/{routingId}/operations/{sequence}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工艺路线编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.RoutingEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行删除工序操作
    /// </summary>
    /// <param name="req">包含工艺路线ID和工序序号的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(RemoveRoutingOperationRequest req, CancellationToken ct)
    {
        // 创建删除工序命令对象
        var cmd = new RemoveRoutingOperationCommand(req.RoutingId, req.Sequence);

        // 通过中介者发送命令，执行实际的删除业务逻辑
        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

