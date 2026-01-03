using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

/// <summary>
/// 取消工单命令
/// </summary>
public record CancelWorkOrderCommand(WorkOrderId WorkOrderId) : ICommand;

/// <summary>
/// 取消工单命令验证器
/// </summary>
public class CancelWorkOrderCommandValidator : AbstractValidator<CancelWorkOrderCommand>
{
    public CancelWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("工单ID不能为空");
    }
}

/// <summary>
/// 取消工单命令处理器
/// </summary>
public class CancelWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository) : ICommandHandler<CancelWorkOrderCommand>
{
    public async Task Handle(CancelWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.WorkOrderId, cancellationToken)
                       ?? throw new KnownException($"未找到工单，WorkOrderId = {request.WorkOrderId}");

        workOrder.Cancel();
    }
}

