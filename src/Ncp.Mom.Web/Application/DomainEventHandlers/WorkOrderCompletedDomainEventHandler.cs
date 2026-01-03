using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

/// <summary>
/// 工单完成后自动释放设备
/// </summary>
public class WorkOrderCompletedDomainEventHandler(
    EquipmentQuery equipmentQuery,
    IEquipmentRepository equipmentRepository,
    ILogger<WorkOrderCompletedDomainEventHandler> logger)
    : IDomainEventHandler<WorkOrderCompletedDomainEvent>
{
    public async Task Handle(
        WorkOrderCompletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var workOrder = notification.WorkOrder;

        // 通过 Query 获取分配给该工单的设备ID列表
        var equipmentIds = await equipmentQuery.GetEquipmentIdsByWorkOrderIdAsync(
            workOrder.Id,
            cancellationToken);

        // 通过 Repository 获取聚合根实例并释放设备
        foreach (var equipmentId in equipmentIds)
        {
            try
            {
                var equipment = await equipmentRepository.GetAsync(equipmentId, cancellationToken);
                if (equipment != null)
                {
                    equipment.Release();
                    logger.LogInformation("工单 {WorkOrderId} 完成后自动释放设备 {EquipmentId}",
                        workOrder.Id, equipment.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "释放设备 {EquipmentId} 失败", equipmentId);
            }
        }
    }
}

