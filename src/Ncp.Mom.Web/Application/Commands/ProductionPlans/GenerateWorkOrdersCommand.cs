using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;
using Ncp.Mom.Web.Application.Queries.Routings;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

/// <summary>
/// 从生产计划生成工单命令
/// </summary>
public record GenerateWorkOrdersCommand(ProductionPlanId ProductionPlanId) : ICommand<IEnumerable<WorkOrderId>>;

/// <summary>
/// 从生产计划生成工单命令验证器
/// </summary>
public class GenerateWorkOrdersCommandValidator : AbstractValidator<GenerateWorkOrdersCommand>
{
    public GenerateWorkOrdersCommandValidator()
    {
        RuleFor(x => x.ProductionPlanId).NotEmpty().WithMessage("生产计划ID不能为空");
    }
}

/// <summary>
/// 从生产计划生成工单命令处理器
/// </summary>
public class GenerateWorkOrdersCommandHandler(
    IProductionPlanRepository productionPlanRepository,
    IWorkOrderRepository workOrderRepository,
    IRoutingRepository routingRepository,
    RoutingQuery routingQuery,
    IMediator mediator,
    ILogger<GenerateWorkOrdersCommandHandler> logger)
    : ICommandHandler<GenerateWorkOrdersCommand, IEnumerable<WorkOrderId>>
{
    public async Task<IEnumerable<WorkOrderId>> Handle(GenerateWorkOrdersCommand request, CancellationToken cancellationToken)
    {
        // 获取生产计划
        var productionPlan = await productionPlanRepository.GetAsync(request.ProductionPlanId, cancellationToken)
                            ?? throw new KnownException($"未找到生产计划，ProductionPlanId = {request.ProductionPlanId}");

        // 验证生产计划状态（只有已审批或进行中的生产计划才能生成工单）
        if (productionPlan.Status != ProductionPlanStatus.Approved && productionPlan.Status != ProductionPlanStatus.InProgress)
        {
            throw new KnownException($"只有已审批或进行中的生产计划才能生成工单，当前状态：{productionPlan.Status}");
        }

        // 查询该产品对应的工艺路线
        var routingQueryInput = new RoutingQueryInput
        {
            ProductId = productionPlan.ProductId,
            PageIndex = 1,
            PageSize = 1
        };
        var routings = await routingQuery.GetRoutingsAsync(routingQueryInput, cancellationToken);

        if (routings.Items == null || !routings.Items.Any())
        {
            throw new KnownException($"产品 {productionPlan.ProductId} 没有对应的工艺路线，无法生成工单");
        }

        // 获取第一个工艺路线（如果有多个，可以扩展逻辑选择）
        var routing = routings.Items.First();
        
        // 获取工艺路线详情以验证其有效性
        var getRoutingQuery = new GetRoutingQuery(routing.Id);
        var routingDetail = await mediator.Send(getRoutingQuery, cancellationToken);

        if (routingDetail.Operations == null || !routingDetail.Operations.Any())
        {
            throw new KnownException($"工艺路线 {routing.RoutingNumber} 没有工序，无法生成工单");
        }

        // 生成工单编号（可以根据业务规则调整）
        var workOrderNumber = $"WO-{productionPlan.PlanNumber}-{DateTime.Now:yyyyMMddHHmmss}";

        // 创建工单
        var workOrder = new WorkOrder(
            workOrderNumber,
            productionPlan.Id,
            productionPlan.ProductId,
            productionPlan.Quantity,
            routing.Id);

        workOrder = await workOrderRepository.AddAsync(workOrder, cancellationToken);

        logger.LogInformation("从生产计划 {ProductionPlanId} 生成工单 {WorkOrderId}，工单编号：{WorkOrderNumber}",
            productionPlan.Id, workOrder.Id, workOrderNumber);

        // 如果生产计划状态是已审批，自动启动生产计划
        if (productionPlan.Status == ProductionPlanStatus.Approved)
        {
            productionPlan.Start();
        }

        return new[] { workOrder.Id };
    }
}

