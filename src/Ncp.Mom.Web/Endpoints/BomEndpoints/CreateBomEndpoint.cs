using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Web.Application.Commands.Boms;
using Ncp.Mom.Web.AppPermissions;

namespace Ncp.Mom.Web.Endpoints.BomEndpoints;

[Tags("Boms")]
public record CreateBomRequest(
    string BomNumber,
    ProductId ProductId,
    int Version);

public record CreateBomResponse(BomId Id);

public class CreateBomEndpoint(IMediator mediator) 
    : Endpoint<CreateBomRequest, ResponseData<CreateBomResponse>>
{
    public override void Configure()
    {
        Post("/api/boms");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.AllApiAccess);
    }

    public override async Task HandleAsync(CreateBomRequest req, CancellationToken ct)
    {
        var cmd = new CreateBomCommand(req.BomNumber, req.ProductId, req.Version);
        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(new CreateBomResponse(id).AsResponseData(), cancellation: ct);
    }
}

