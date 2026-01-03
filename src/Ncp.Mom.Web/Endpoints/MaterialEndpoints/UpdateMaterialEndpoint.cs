using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Materials;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

[Tags("Materials")]
public record UpdateMaterialRequest
{
    public MaterialId Id { get; set; } = default!;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Unit { get; set; }
}

public class UpdateMaterialEndpoint(IMediator mediator) : Endpoint<UpdateMaterialRequest>
{
    public override void Configure()
    {
        Put("/api/materials/{id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(UpdateMaterialRequest req, CancellationToken ct)
    {
        var cmd = new UpdateMaterialCommand(
            req.Id,
            req.MaterialCode,
            req.MaterialName,
            req.Specification,
            req.Unit);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

