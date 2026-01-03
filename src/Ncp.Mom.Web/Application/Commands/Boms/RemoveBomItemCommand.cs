using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Boms;

public record RemoveBomItemCommand(
    BomId BomId,
    BomItemId ItemId) : ICommand;

public class RemoveBomItemCommandHandler(
    IBomRepository bomRepository) 
    : ICommandHandler<RemoveBomItemCommand>
{
    public async Task Handle(
        RemoveBomItemCommand request, 
        CancellationToken cancellationToken)
    {
        var bom = await bomRepository.GetAsync(
            request.BomId, 
            cancellationToken)
            ?? throw new KnownException("BOM不存在");

        bom.RemoveItem(request.ItemId);
    }
}

