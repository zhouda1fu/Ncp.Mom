using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.ProductionPlans;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

/// <summary>
/// 从生产计划生成工单的请求模型
/// </summary>
/// <param name="Id">生产计划ID</param>
public record GenerateWorkOrdersRequest(ProductionPlanId Id);

/// <summary>
/// 从生产计划生成工单的响应模型
/// </summary>
/// <param name="ProductionPlanId">生产计划ID</param>
/// <param name="WorkOrderIds">生成的工单ID列表</param>
public record GenerateWorkOrdersResponse(ProductionPlanId ProductionPlanId, IEnumerable<WorkOrderId> WorkOrderIds);

/// <summary>
/// 从生产计划生成工单的API端点
/// 该端点用于根据生产计划自动生成工单
/// </summary>
[Tags("ProductionPlans")]
public class GenerateWorkOrdersEndpoint(IMediator mediator) : Endpoint<GenerateWorkOrdersRequest, ResponseData<GenerateWorkOrdersResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP POST方法，用于生成工单
        Post("/api/production-plans/{id}/generate-work-orders");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和生产计划编辑权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.ProductionPlanEdit);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行生成工单操作
    /// </summary>
    /// <param name="req">包含生产计划ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(GenerateWorkOrdersRequest req, CancellationToken ct)
    {
        // 创建生成工单命令对象
        var cmd = new GenerateWorkOrdersCommand(req.Id);

        // 通过中介者发送命令，执行实际的生成业务逻辑
        var workOrderIds = await mediator.Send(cmd, ct);

        // 创建响应对象
        var response = new GenerateWorkOrdersResponse(req.Id, workOrderIds);

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

