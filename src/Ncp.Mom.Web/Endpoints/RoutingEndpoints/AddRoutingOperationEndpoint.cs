using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Commands.Routings;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

/// <summary>
/// 添加工艺路线工序的请求模型
/// </summary>
public record AddRoutingOperationRequest
{
    public RoutingId RoutingId { get; set; } = default!;
    public int Sequence { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public WorkCenterId WorkCenterId { get; set; } = default!;
    public decimal StandardTime { get; set; }
}

/// <summary>
/// 添加工艺路线工序的API端点
/// 该端点用于向工艺路线中添加新的工序
/// </summary>
[Tags("Routings")]
public class AddRoutingOperationEndpoint(IMediator mediator)
    : Endpoint<AddRoutingOperationRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP POST方法，用于添加工序
        Post("/api/routings/{routingId}/operations");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工艺路线编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.RoutingEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行添加工序操作
    /// </summary>
    /// <param name="req">包含工序信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(
        AddRoutingOperationRequest req,
        CancellationToken ct)
    {
        var cmd = new AddRoutingOperationCommand(
            req.RoutingId,
            req.Sequence,
            req.OperationName,
            req.WorkCenterId,
            req.StandardTime);

        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

