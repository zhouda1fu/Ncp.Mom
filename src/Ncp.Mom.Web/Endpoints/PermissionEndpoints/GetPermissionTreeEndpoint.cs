using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.PermissionEndpoints;

/// <summary>
/// 获取权限树形结构的API端点
/// 该端点用于查询系统中所有权限的分组树形结构，便于前端展示权限管理界面
/// </summary>
[Tags("Permissions")]
public class GetPermissionTreeEndpoint : EndpointWithoutRequest<ResponseData<IEnumerable<AppPermissionGroup>>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP GET方法，用于查询权限树形结构
        Get("/api/permissions/tree");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限和用户角色分配权限
        Permissions(PermissionCodes.AllApiAccess, PermissionCodes.UserRoleAssign);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 从权限定义上下文中获取权限分组信息并返回结果
    /// </summary>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        // 从权限定义上下文中获取所有权限分组信息
        // 这些分组信息在应用启动时就已经配置好
        var permissionGroups = PermissionDefinitionContext.PermissionGroups;

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(new ResponseData<IEnumerable<AppPermissionGroup>>(permissionGroups), cancellation: ct);
    }
}

