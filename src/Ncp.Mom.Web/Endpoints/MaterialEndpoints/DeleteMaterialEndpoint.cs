using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Materials;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

[Tags("Materials")]
public record DeleteMaterialRequest { public MaterialId Id { get; set; } = default!; }

public class DeleteMaterialEndpoint(IMediator mediator) : Endpoint<DeleteMaterialRequest>
{
    public override void Configure()
    {
        Delete("/api/materials/{id}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(DeleteMaterialRequest req, CancellationToken ct)
    {
        var cmd = new DeleteMaterialCommand(req.Id);
        await mediator.Send(cmd, ct);
        await Send.OkAsync(cancellation: ct);
    }
}

