using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Equipments;

public record StartMaintenanceCommand(EquipmentId EquipmentId) : ICommand;

public class StartMaintenanceCommandHandler(
    IEquipmentRepository equipmentRepository) 
    : ICommandHandler<StartMaintenanceCommand>
{
    public async Task Handle(
        StartMaintenanceCommand request, 
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.GetAsync(
            request.EquipmentId, 
            cancellationToken)
            ?? throw new KnownException("设备不存在");

        equipment.StartMaintenance();
    }
}

