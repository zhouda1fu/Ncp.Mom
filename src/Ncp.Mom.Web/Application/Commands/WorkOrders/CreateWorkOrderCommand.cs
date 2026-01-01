using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

public record CreateWorkOrderCommand(
    string WorkOrderNumber,
    ProductionPlanId ProductionPlanId,
    ProductId ProductId,
    int Quantity,
    RoutingId RoutingId) : ICommand<WorkOrderId>;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator(WorkOrderQuery workOrderQuery)
    {
        RuleFor(x => x.WorkOrderNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductionPlanId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.RoutingId).NotEmpty();
        RuleFor(x => x.WorkOrderNumber)
            .MustAsync(async (workOrderNumber, ct) => !await workOrderQuery.DoesWorkOrderExist(workOrderNumber, ct))
            .WithMessage(x => $"工单编号 {x.WorkOrderNumber} 已存在");
    }
}

public class CreateWorkOrderCommandHandler(
    IWorkOrderRepository workOrderRepository,
    ILogger<CreateWorkOrderCommandHandler> logger)
    : ICommandHandler<CreateWorkOrderCommand, WorkOrderId>
{
    public async Task<WorkOrderId> Handle(
        CreateWorkOrderCommand request,
        CancellationToken cancellationToken)
    {
        var workOrder = new WorkOrder(
            request.WorkOrderNumber,
            request.ProductionPlanId,
            request.ProductId,
            request.Quantity,
            request.RoutingId);

        workOrder = await workOrderRepository.AddAsync(workOrder, cancellationToken);
        logger.LogInformation("工单已创建，ID: {WorkOrderId}, 工单编号: {WorkOrderNumber}",
            workOrder.Id, workOrder.WorkOrderNumber);
        return workOrder.Id;
    }
}

