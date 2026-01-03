using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

/// <summary>
/// 暂停工单命令
/// </summary>
public record PauseWorkOrderCommand(WorkOrderId WorkOrderId) : ICommand;

/// <summary>
/// 暂停工单命令验证器
/// </summary>
public class PauseWorkOrderCommandValidator : AbstractValidator<PauseWorkOrderCommand>
{
    public PauseWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("工单ID不能为空");
    }
}

/// <summary>
/// 暂停工单命令处理器
/// </summary>
public class PauseWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository) : ICommandHandler<PauseWorkOrderCommand>
{
    public async Task Handle(PauseWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.WorkOrderId, cancellationToken)
                       ?? throw new KnownException($"未找到工单，WorkOrderId = {request.WorkOrderId}");

        workOrder.Pause();
    }
}

