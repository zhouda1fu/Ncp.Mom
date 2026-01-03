using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

/// <summary>
/// 更新工艺路线的请求模型
/// </summary>
/// <param name="RoutingId">工艺路线ID</param>
/// <param name="RoutingNumber">工艺路线编码</param>
/// <param name="Name">工艺路线名称</param>
public record UpdateRoutingRequest(RoutingId RoutingId, string RoutingNumber, string Name);

/// <summary>
/// 更新工艺路线的响应模型
/// </summary>
/// <param name="RoutingId">已更新的工艺路线ID</param>
public record UpdateRoutingResponse(RoutingId RoutingId);

/// <summary>
/// 更新工艺路线的API端点
/// 该端点用于修改现有工艺路线的基本信息
/// </summary>
[Tags("Routings")]
public class UpdateRoutingEndpoint(IMediator mediator) : Endpoint<UpdateRoutingRequest, ResponseData<UpdateRoutingResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP PUT方法，用于更新工艺路线信息
        Put("/api/routings/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工艺路线编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.RoutingEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，并返回更新结果
    /// </summary>
    /// <param name="req">包含工艺路线更新信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(UpdateRoutingRequest req, CancellationToken ct)
    {
        // 创建更新工艺路线命令对象
        var cmd = new UpdateRoutingCommand(req.RoutingId, req.RoutingNumber, req.Name);

        // 通过中介者发送命令，执行实际的更新业务逻辑
        await mediator.Send(cmd, ct);

        // 创建响应对象，包含已更新的工艺路线ID
        var response = new UpdateRoutingResponse(req.RoutingId);

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

