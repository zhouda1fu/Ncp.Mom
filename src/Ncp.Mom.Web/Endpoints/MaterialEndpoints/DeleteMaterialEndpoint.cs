using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Materials;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

/// <summary>
/// 删除物料的请求模型
/// </summary>
/// <param name="Id">物料ID</param>
[Tags("Materials")]
public record DeleteMaterialRequest(MaterialId Id);

/// <summary>
/// 删除物料的API端点
/// 该端点用于删除物料
/// </summary>
public class DeleteMaterialEndpoint(IMediator mediator) : Endpoint<DeleteMaterialRequest, ResponseData<bool>>
{
    /// <summary>
    /// 配置端点的基本设置
    /// 包括HTTP方法、认证方案、权限要求等
    /// </summary>
    public override void Configure()
    {
        Delete("/api/materials/{id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    /// <summary>
    /// 处理HTTP请求的核心方法
    /// 将请求转换为命令，通过中介者发送，执行删除物料操作
    /// </summary>
    /// <param name="req">包含物料ID的请求对象</param>
    /// <param name="ct">取消令牌，用于支持异步操作的取消</param>
    /// <returns>异步任务</returns>
    public override async Task HandleAsync(DeleteMaterialRequest req, CancellationToken ct)
    {
        var cmd = new DeleteMaterialCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(true.AsResponseData(), cancellation: ct);
    }
}

