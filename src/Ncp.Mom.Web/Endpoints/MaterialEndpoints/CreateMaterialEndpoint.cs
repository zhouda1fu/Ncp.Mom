using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Web.Application.Commands.Materials;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.MaterialEndpoints;

[Tags("Materials")]
public record CreateMaterialRequest(
    string MaterialCode,
    string MaterialName,
    string? Specification = null,
    string? Unit = null);

public record CreateMaterialResponse(MaterialId Id);

public class CreateMaterialEndpoint(IMediator mediator) 
    : Endpoint<CreateMaterialRequest, ResponseData<CreateMaterialResponse>>
{
    public override void Configure()
    {
        Post("/api/materials");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(CreateMaterialRequest req, CancellationToken ct)
    {
        var cmd = new CreateMaterialCommand(
            req.MaterialCode,
            req.MaterialName,
            req.Specification,
            req.Unit);
        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(new CreateMaterialResponse(id).AsResponseData(), cancellation: ct);
    }
}

