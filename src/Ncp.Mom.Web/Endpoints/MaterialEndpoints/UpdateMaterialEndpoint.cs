using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Materials;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

/// <summary>
/// 更新物料的请求模型
/// </summary>
[Tags("Materials")]
public record UpdateMaterialRequest
{
    public MaterialId Id { get; set; } = default!;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Unit { get; set; }
}

/// <summary>
/// 更新物料的API端点
/// 该端点用于更新物料信息
/// </summary>
public class UpdateMaterialEndpoint(IMediator mediator) : Endpoint<UpdateMaterialRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Put("/api/materials/{id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行更新物料操作
    /// </summary>
    /// <param name="req">包含物料更新信息的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(UpdateMaterialRequest req, CancellationToken ct)
    {
        var cmd = new UpdateMaterialCommand(
            req.Id,
            req.MaterialCode,
            req.MaterialName,
            req.Specification,
            req.Unit);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

