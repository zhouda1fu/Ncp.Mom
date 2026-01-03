using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Commands.QualityInspections;

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

/// <summary>
/// 工单创建后自动创建质检单
/// </summary>
public class WorkOrderCreatedDomainEventHandler(
    IMediator mediator,
    ILogger<WorkOrderCreatedDomainEventHandler> logger)
    : IDomainEventHandler<WorkOrderCreatedDomainEvent>
{
    public async Task Handle(
        WorkOrderCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var workOrder = notification.WorkOrder;

        // 计算抽样数量（根据业务规则，这里使用简单的10%抽样，最少1个）
        var sampleQuantity = CalculateSampleQuantity(workOrder.Quantity);

        // 创建质检单编号
        var inspectionNumber = $"QC-{workOrder.WorkOrderNumber}";

        // 创建质检单
        var createInspectionCommand = new CreateQualityInspectionCommand(
            inspectionNumber,
            workOrder.Id,
            sampleQuantity);

        var inspectionId = await mediator.Send(createInspectionCommand, cancellationToken);

        logger.LogInformation("工单 {WorkOrderId} 创建后自动创建质检单 {InspectionId}，抽样数量：{SampleQuantity}",
            workOrder.Id, inspectionId, sampleQuantity);
    }

    /// <summary>
    /// 计算抽样数量
    /// </summary>
    private int CalculateSampleQuantity(int totalQuantity)
    {
        // 根据抽样规则计算抽样数量
        // 如果总数大于等于100，抽样10%，否则全部检验
        if (totalQuantity >= 100)
        {
            return Math.Max(1, (int)(totalQuantity * 0.1));
        }
        return totalQuantity;
    }
}

