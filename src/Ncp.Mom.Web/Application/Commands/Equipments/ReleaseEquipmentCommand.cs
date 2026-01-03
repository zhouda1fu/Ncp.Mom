using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Equipments;

public record ReleaseEquipmentCommand(EquipmentId EquipmentId) : ICommand;

public class ReleaseEquipmentCommandHandler(
    IEquipmentRepository equipmentRepository) 
    : ICommandHandler<ReleaseEquipmentCommand>
{
    public async Task Handle(
        ReleaseEquipmentCommand request, 
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.GetAsync(
            request.EquipmentId, 
            cancellationToken)
            ?? throw new KnownException("设备不存在");

        equipment.Release();
    }
}

