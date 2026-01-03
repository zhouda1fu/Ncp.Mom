using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.WorkCenterEndpoints;

/// <summary>
/// 获取单个工作中心的请求模型
/// </summary>
/// <param name="Id">工作中心ID</param>
public record GetWorkCenterRequest(WorkCenterId Id);

/// <summary>
/// 获取单个工作中心的响应模型
/// </summary>
/// <param name="Id">工作中心ID</param>
/// <param name="WorkCenterCode">工作中心编码</param>
/// <param name="WorkCenterName">工作中心名称</param>
public record GetWorkCenterResponse(WorkCenterId Id, string WorkCenterCode, string WorkCenterName);

/// <summary>
/// 获取单个工作中心的API端点
/// 该端点用于根据ID查询特定工作中心的详细信息
/// </summary>
[Tags("WorkCenters")]
public class GetWorkCenterEndpoint(WorkCenterQuery workCenterQuery) : Endpoint<GetWorkCenterRequest, ResponseData<GetWorkCenterResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP GET方法，通过路由参数获取工作中心ID
        Get("/api/work-centers/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 根据工作中心ID查询详细信息并返回结果
    /// </summary>
    /// <param name="req">包含工作中心ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(GetWorkCenterRequest req, CancellationToken ct)
    {
        // 通过查询服务获取工作中心详细信息
        var workCenter = await workCenterQuery.GetWorkCenterByIdAsync(req.Id, ct);

        // 验证工作中心是否存在
        if (workCenter == null)
        {
            throw new KnownException($"未找到工作中心，Id = {req.Id}");
        }

        // 创建响应对象
        var response = new GetWorkCenterResponse(
            workCenter.Id,
            workCenter.WorkCenterCode,
            workCenter.WorkCenterName
        );

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

