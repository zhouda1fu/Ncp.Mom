using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Equipments;

public record AssignEquipmentCommand(
    EquipmentId EquipmentId,
    WorkOrderId WorkOrderId) : ICommand;

public class AssignEquipmentCommandHandler(
    IEquipmentRepository equipmentRepository) 
    : ICommandHandler<AssignEquipmentCommand>
{
    public async Task Handle(
        AssignEquipmentCommand request, 
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.GetAsync(
            request.EquipmentId, 
            cancellationToken)
            ?? throw new KnownException("设备不存在");

        equipment.AssignToWorkOrder(request.WorkOrderId);
    }
}

