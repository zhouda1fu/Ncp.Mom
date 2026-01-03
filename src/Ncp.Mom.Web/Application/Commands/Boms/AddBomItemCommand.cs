using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Boms;

public record AddBomItemCommand(
    BomId BomId,
    MaterialId MaterialId,
    decimal Quantity,
    string Unit) : ICommand;

public class AddBomItemCommandHandler(
    IBomRepository bomRepository) 
    : ICommandHandler<AddBomItemCommand>
{
    public async Task Handle(
        AddBomItemCommand request, 
        CancellationToken cancellationToken)
    {
        var bom = await bomRepository.GetAsync(
            request.BomId, 
            cancellationToken)
            ?? throw new KnownException("BOM不存在");

        bom.AddItem(request.MaterialId, request.Quantity, request.Unit);
    }
}

