using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

/// <summary>
/// 添加BOM项的请求模型
/// </summary>
public record AddBomItemRequest
{
    public BomId Id { get; set; } = default!;
    public MaterialId MaterialId { get; set; } = default!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

/// <summary>
/// 添加BOM项的API端点
/// 该端点用于向BOM中添加物料项
/// </summary>
[Tags("Boms")]
public class AddBomItemEndpoint(IMediator mediator) : Endpoint<AddBomItemRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Post("/api/boms/{id}/items");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行添加BOM项操作
    /// </summary>
    /// <param name="req">包含BOM项信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(AddBomItemRequest req, CancellationToken ct)
    {
        var cmd = new AddBomItemCommand(req.Id, req.MaterialId, req.Quantity, req.Unit);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

