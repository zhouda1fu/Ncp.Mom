using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Equipments;

public record CompleteMaintenanceCommand(EquipmentId EquipmentId) : ICommand;

public class CompleteMaintenanceCommandHandler(
    IEquipmentRepository equipmentRepository) 
    : ICommandHandler<CompleteMaintenanceCommand>
{
    public async Task Handle(
        CompleteMaintenanceCommand request, 
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.GetAsync(
            request.EquipmentId, 
            cancellationToken)
            ?? throw new KnownException("设备不存在");

        equipment.CompleteMaintenance();
    }
}

