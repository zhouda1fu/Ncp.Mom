using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Commands.WorkCenters;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.WorkCenterEndpoints;

/// <summary>
/// 更新工作中心的请求模型
/// </summary>
/// <param name="WorkCenterId">工作中心ID</param>
/// <param name="WorkCenterCode">工作中心编码</param>
/// <param name="WorkCenterName">工作中心名称</param>
public record UpdateWorkCenterRequest(WorkCenterId WorkCenterId, string WorkCenterCode, string WorkCenterName);

/// <summary>
/// 更新工作中心的响应模型
/// </summary>
/// <param name="WorkCenterId">已更新的工作中心ID</param>
public record UpdateWorkCenterResponse(WorkCenterId WorkCenterId);

/// <summary>
/// 更新工作中心的API端点
/// 该端点用于修改现有工作中心的基本信息
/// </summary>
[Tags("WorkCenters")]
public class UpdateWorkCenterEndpoint(IMediator mediator) : Endpoint<UpdateWorkCenterRequest, ResponseData<UpdateWorkCenterResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP PUT方法，用于更新工作中心信息
        Put("/api/work-centers/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，并返回更新结果
    /// </summary>
    /// <param name="req">包含工作中心更新信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(UpdateWorkCenterRequest req, CancellationToken ct)
    {
        // 创建更新工作中心命令对象
        var cmd = new UpdateWorkCenterCommand(req.WorkCenterId, req.WorkCenterCode, req.WorkCenterName);

        // 通过中介者发送命令，执行实际的更新业务逻辑
        await mediator.Send(cmd, ct);

        // 创建响应对象，包含已更新的工作中心ID
        var response = new UpdateWorkCenterResponse(req.WorkCenterId);

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

