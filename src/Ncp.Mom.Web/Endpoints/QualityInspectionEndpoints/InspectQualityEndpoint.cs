using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Web.Application.Commands.QualityInspections;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.QualityInspectionEndpoints;

/// <summary>
/// 执行质检的请求模型
/// </summary>
public record InspectQualityRequest
{
    public QualityInspectionId Id { get; set; } = default!;
    public int QualifiedQuantity { get; set; }
    public int UnqualifiedQuantity { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 执行质检的API端点
/// </summary>
[Tags("QualityInspections")]
public class InspectQualityEndpoint(IMediator mediator) 
    : Endpoint<InspectQualityRequest>
{
    public override void Configure()
    {
        Post("/api/quality-inspections/{id}/inspect");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(InspectQualityRequest req, CancellationToken ct)
    {
        var cmd = new InspectQualityCommand(
            req.Id,
            req.QualifiedQuantity,
            req.UnqualifiedQuantity,
            req.Remark);

        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

