using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

/// <summary>
/// 删除工艺路线的请求模型
/// </summary>
/// <param name="RoutingId">要删除的工艺路线ID</param>
public record DeleteRoutingRequest(RoutingId RoutingId);

/// <summary>
/// 删除工艺路线的API端点
/// 该端点用于从系统中删除指定的工艺路线
/// </summary>
[Tags("Routings")]
public class DeleteRoutingEndpoint(IMediator mediator) : Endpoint<DeleteRoutingRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP DELETE方法，用于删除工艺路线
        Delete("/api/routings/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工艺路线删除权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.RoutingDelete);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行删除操作
    /// </summary>
    /// <param name="req">包含工艺路线ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(DeleteRoutingRequest req, CancellationToken ct)
    {
        // 创建删除工艺路线命令对象
        var cmd = new DeleteRoutingCommand(req.RoutingId);

        // 通过中介者发送命令，执行实际的删除业务逻辑
        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

