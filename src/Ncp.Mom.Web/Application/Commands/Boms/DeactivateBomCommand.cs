using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Boms;

public record DeactivateBomCommand(BomId BomId) : ICommand;

public class DeactivateBomCommandHandler(
    IBomRepository bomRepository) 
    : ICommandHandler<DeactivateBomCommand>
{
    public async Task Handle(
        DeactivateBomCommand request, 
        CancellationToken cancellationToken)
    {
        var bom = await bomRepository.GetAsync(
            request.BomId, 
            cancellationToken)
            ?? throw new KnownException("BOM不存在");

        bom.Deactivate();
    }
}

