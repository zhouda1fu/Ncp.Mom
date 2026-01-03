using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Commands.Products;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.ProductEndpoints;

/// <summary>
/// 更新产品的请求模型
/// </summary>
/// <param name="ProductId">产品ID</param>
/// <param name="ProductCode">产品编码</param>
/// <param name="ProductName">产品名称</param>
public record UpdateProductRequest(ProductId ProductId, string ProductCode, string ProductName);

/// <summary>
/// 更新产品的响应模型
/// </summary>
/// <param name="ProductId">已更新的产品ID</param>
public record UpdateProductResponse(ProductId ProductId);

/// <summary>
/// 更新产品的API端点
/// 该端点用于修改现有产品的基本信息
/// </summary>
[Tags("Products")]
public class UpdateProductEndpoint(IMediator mediator) : Endpoint<UpdateProductRequest, ResponseData<UpdateProductResponse>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP PUT方法，用于更新产品信息
        Put("/api/products/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，并返回更新结果
    /// </summary>
    /// <param name="req">包含产品更新信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        // 创建更新产品命令对象
        var cmd = new UpdateProductCommand(req.ProductId, req.ProductCode, req.ProductName);

        // 通过中介者发送命令，执行实际的更新业务逻辑
        await mediator.Send(cmd, ct);

        // 创建响应对象，包含已更新的产品ID
        var response = new UpdateProductResponse(req.ProductId);

        // 返回成功响应，使用统一的响应数据格式包装
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}

