using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Web.Application.Commands.WorkOrders;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

/// <summary>
/// 生产计划启动后自动创建工单
/// </summary>
public class ProductionPlanStartedDomainEventHandler(
    IMediator mediator,
    IRoutingRepository routingRepository,
    ILogger<ProductionPlanStartedDomainEventHandler> logger)
    : IDomainEventHandler<ProductionPlanStartedDomainEvent>
{
    public async Task Handle(
        ProductionPlanStartedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var plan = notification.ProductionPlan;

        // 根据产品ID查找工艺路线
        var routings = await routingRepository.GetByProductIdAsync(
            plan.ProductId, cancellationToken);

        if (routings.Count == 0)
        {
            logger.LogWarning("产品 {ProductId} 没有找到工艺路线，无法创建工单", plan.ProductId);
            return;
        }

        // 使用第一个激活的工艺路线（实际业务中可能需要更复杂的逻辑）
        var routing = routings.FirstOrDefault();

        if (routing == null)
        {
            logger.LogWarning("产品 {ProductId} 没有可用的工艺路线", plan.ProductId);
            return;
        }

        // 创建工单
        var workOrderNumber = $"WO-{plan.PlanNumber}-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
        var createWorkOrderCommand = new CreateWorkOrderCommand(
            workOrderNumber,
            plan.Id,
            plan.ProductId,
            plan.Quantity,
            routing.Id);

        var workOrderId = await mediator.Send(createWorkOrderCommand, cancellationToken);

        logger.LogInformation("生产计划 {ProductionPlanId} 启动后自动创建工单 {WorkOrderId}",
            plan.Id, workOrderId);
    }
}

