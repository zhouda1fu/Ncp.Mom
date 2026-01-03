using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Commands.ProductionPlans;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

/// <summary>
/// 完成生产计划的请求模型
/// </summary>
/// <param name="Id">生产计划ID</param>
public record CompleteProductionPlanRequest(ProductionPlanId Id);

/// <summary>
/// 完成生产计划的API端点
/// 该端点用于将进行中的生产计划标记为已完成
/// </summary>
[Tags("ProductionPlans")]
public class CompleteProductionPlanEndpoint(IMediator mediator) : Endpoint<CompleteProductionPlanRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP POST方法，用于完成生产计划
        Post("/api/production-plans/{id}/complete");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和生产计划编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.ProductionPlanEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行完成操作
    /// </summary>
    /// <param name="req">包含生产计划ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(CompleteProductionPlanRequest req, CancellationToken ct)
    {
        // 创建完成生产计划命令对象
        var cmd = new CompleteProductionPlanCommand(req.Id);

        // 通过中介者发送命令，执行实际的完成业务逻辑
        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

