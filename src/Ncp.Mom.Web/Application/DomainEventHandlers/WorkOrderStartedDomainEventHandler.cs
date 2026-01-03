using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.Equipments;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

/// <summary>
/// 工单启动后自动分配设备
/// </summary>
public class WorkOrderStartedDomainEventHandler(
    IMediator mediator,
    IRoutingRepository routingRepository,
    EquipmentQuery equipmentQuery,
    ILogger<WorkOrderStartedDomainEventHandler> logger)
    : IDomainEventHandler<WorkOrderStartedDomainEvent>
{
    public async Task Handle(
        WorkOrderStartedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var workOrder = notification.WorkOrder;

        // 获取工艺路线
        var routing = await routingRepository.GetAsync(workOrder.RoutingId, cancellationToken);
        if (routing == null || routing.Operations.Count == 0)
        {
            logger.LogWarning("工单 {WorkOrderId} 的工艺路线不存在或没有工序，无法分配设备", workOrder.Id);
            return;
        }

        // 获取第一个工序的工作中心
        var firstOperation = routing.Operations.OrderBy(o => o.Sequence).First();
        
        // 查询可用设备
        var availableEquipments = await equipmentQuery.GetAvailableEquipmentsAsync(
            firstOperation.WorkCenterId,
            cancellationToken);

        if (availableEquipments.Count == 0)
        {
            logger.LogWarning("工作中心 {WorkCenterId} 没有可用设备，无法分配给工单 {WorkOrderId}",
                firstOperation.WorkCenterId, workOrder.Id);
            return;
        }

        // 分配第一个可用设备
        var equipment = availableEquipments.First();
        var assignCommand = new AssignEquipmentCommand(equipment.Id, workOrder.Id);
        await mediator.Send(assignCommand, cancellationToken);

        logger.LogInformation("工单 {WorkOrderId} 启动后自动分配设备 {EquipmentId}",
            workOrder.Id, equipment.Id);
    }
}

