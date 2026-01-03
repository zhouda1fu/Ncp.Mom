using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

/// <summary>
/// 恢复工单命令
/// </summary>
public record ResumeWorkOrderCommand(WorkOrderId WorkOrderId) : ICommand;

/// <summary>
/// 恢复工单命令验证器
/// </summary>
public class ResumeWorkOrderCommandValidator : AbstractValidator<ResumeWorkOrderCommand>
{
    public ResumeWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("工单ID不能为空");
    }
}

/// <summary>
/// 恢复工单命令处理器
/// </summary>
public class ResumeWorkOrderCommandHandler(IWorkOrderRepository workOrderRepository) : ICommandHandler<ResumeWorkOrderCommand>
{
    public async Task Handle(ResumeWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.WorkOrderId, cancellationToken)
                       ?? throw new KnownException($"未找到工单，WorkOrderId = {request.WorkOrderId}");

        workOrder.Resume();
    }
}

