using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Commands.Products;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.ProductEndpoints;

/// <summary>
/// 删除产品的请求模型
/// </summary>
/// <param name="ProductId">要删除的产品ID</param>
public record DeleteProductRequest(ProductId ProductId);

/// <summary>
/// 删除产品的API端点
/// 该端点用于从系统中删除指定的产品
/// </summary>
[Tags("Products")]
public class DeleteProductEndpoint(IMediator mediator) : Endpoint<DeleteProductRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        // 设置HTTP DELETE方法，用于删除产品
        Delete("/api/products/{id}");

        // 设置JWT Bearer认证方案，要求用户必须提供有效的JWT令牌
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        // 设置权限要求：用户必须同时拥有API访问权限
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行删除操作
    /// </summary>
    /// <param name="req">包含产品ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        // 创建删除产品命令对象
        var cmd = new DeleteProductCommand(req.ProductId);

        // 通过中介者发送命令，执行实际的删除业务逻辑
        await mediator.Send(cmd, ct);

        // 返回成功响应
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

