using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Commands.WorkCenters;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.WorkCenterEndpoints;

/// <summary>
/// 创建工作中心的请求模型
/// </summary>
/// <param name="WorkCenterCode">工作中心编码</param>
/// <param name="WorkCenterName">工作中心名称</param>
public record CreateWorkCenterRequest(string WorkCenterCode, string WorkCenterName);

/// <summary>
/// 创建工作中心的响应模型
/// </summary>
/// <param name="WorkCenterId">新创建的工作中心ID</param>
/// <param name="WorkCenterCode">工作中心编码</param>
/// <param name="WorkCenterName">工作中心名称</param>
public record CreateWorkCenterResponse(WorkCenterId WorkCenterId, string WorkCenterCode, string WorkCenterName);

/// <summary>
/// 创建工作中心的API端点
/// 该端点用于在系统中创建新的工作中心
/// </summary>
[Tags("WorkCenters")]
public class CreateWorkCenterEndpoint(IMediator mediator) : Endpoint<CreateWorkCenterRequest, ResponseData<CreateWorkCenterResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP POST方法，用于创建新的工作中心
        Post("/api/work-centers");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，并返回新创建的工作中心信息
    /// </summary>
    /// <param name="req">包含工作中心基本信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(CreateWorkCenterRequest req, CancellationToken ct)
    {
        // 将请求转换为领域命令对象
        var cmd = new CreateWorkCenterCommand(req.WorkCenterCode, req.WorkCenterName);

        // 通过中介者发送命令，执行实际的业务逻辑
        // 返回新创建的工作中心ID
        var result = await mediator.Send(cmd, ct);

        // 创建响应对象，包含新创建的工作中心信息
        var response = new CreateWorkCenterResponse(
            result,              // 新创建的工作中心ID
            req.WorkCenterCode,  // 工作中心编码
            req.WorkCenterName   // 工作中心名称
        );

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

