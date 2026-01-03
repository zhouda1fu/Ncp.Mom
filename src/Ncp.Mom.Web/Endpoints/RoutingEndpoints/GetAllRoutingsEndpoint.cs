using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.RoutingEndpoints;

/// <summary>
/// 获取所有工艺路线的API端点
/// 该端点用于查询系统中的所有工艺路线信息，支持分页、搜索和产品筛选
/// </summary>
[Tags("Routings")]
public class GetAllRoutingsEndpoint(RoutingQuery routingQuery) : Endpoint<RoutingQueryInput, ResponseData<PagedData<RoutingQueryDto>>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP GET方法，用于查询工艺路线信息
        Get("/api/routings");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和工艺路线查看权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.RoutingView);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 查询所有工艺路线信息并返回分页结果
    /// </summary>
    /// <param name="req">工艺路线查询输入参数，包含分页、搜索和产品筛选条件</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(RoutingQueryInput req, CancellationToken ct)
    {
        // 通过查询服务获取所有工艺路线信息
        var routings = await routingQuery.GetRoutingsAsync(req, ct);

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(routings.AsResponseData(), cancellation: ct);
    }
}

