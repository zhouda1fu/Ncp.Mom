using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Materials;

public record UpdateMaterialCommand(
    MaterialId MaterialId,
    string MaterialCode,
    string MaterialName,
    string? Specification = null,
    string? Unit = null) : ICommand;

public class UpdateMaterialCommandHandler(
    IMaterialRepository materialRepository) 
    : ICommandHandler<UpdateMaterialCommand>
{
    public async Task Handle(
        UpdateMaterialCommand request, 
        CancellationToken cancellationToken)
    {
        var material = await materialRepository.GetAsync(
            request.MaterialId, 
            cancellationToken)
            ?? throw new KnownException("物料不存在");

        material.UpdateInfo(
            request.MaterialCode,
            request.MaterialName,
            request.Specification,
            request.Unit);
    }
}

