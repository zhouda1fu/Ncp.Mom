using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.ProductEndpoints;

/// <summary>
/// 获取单个产品的请求模型
/// </summary>
/// <param name="Id">产品ID</param>
public record GetProductRequest(ProductId Id);

/// <summary>
/// 获取单个产品的响应模型
/// </summary>
/// <param name="Id">产品ID</param>
/// <param name="ProductCode">产品编码</param>
/// <param name="ProductName">产品名称</param>
public record GetProductResponse(ProductId Id, string ProductCode, string ProductName);

/// <summary>
/// 获取单个产品的API端点
/// 该端点用于根据ID查询特定产品的详细信息
/// </summary>
[Tags("Products")]
public class GetProductEndpoint(ProductQuery productQuery) : Endpoint<GetProductRequest, ResponseData<GetProductResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP GET方法，通过路由参数获取产品ID
        Get("/api/products/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 根据产品ID查询详细信息并返回结果
    /// </summary>
    /// <param name="req">包含产品ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        // 通过查询服务获取产品详细信息
        var product = await productQuery.GetProductByIdAsync(req.Id, ct);

        // 验证产品是否存在
        if (product == null)
        {
            throw new KnownException($"未找到产品，Id = {req.Id}");
        }

        // 创建响应对象
        var response = new GetProductResponse(
            product.Id,
            product.ProductCode,
            product.ProductName
        );

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

