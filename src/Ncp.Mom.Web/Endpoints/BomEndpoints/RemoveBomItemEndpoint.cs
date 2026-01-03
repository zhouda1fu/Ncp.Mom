using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

/// <summary>
/// 删除BOM项的请求模型
/// </summary>
public record RemoveBomItemRequest
{
    public BomId Id { get; set; } = default!;
    public BomItemId ItemId { get; set; } = default!;
}

/// <summary>
/// 删除BOM项的API端点
/// 该端点用于从BOM中删除物料项
/// </summary>
[Tags("Boms")]
public class RemoveBomItemEndpoint(IMediator mediator) : Endpoint<RemoveBomItemRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Delete("/api/boms/{id}/items/{itemId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行删除BOM项操作
    /// </summary>
    /// <param name="req">包含BOM ID和物料项ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(RemoveBomItemRequest req, CancellationToken ct)
    {
        var cmd = new RemoveBomItemCommand(req.Id, req.ItemId);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

