using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

public record StartWorkOrderCommand(WorkOrderId Id) : ICommand;

public class StartWorkOrderCommandValidator : AbstractValidator<StartWorkOrderCommand>
{
    public StartWorkOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class StartWorkOrderCommandHandler(
    IWorkOrderRepository workOrderRepository,
    ILogger<StartWorkOrderCommandHandler> logger)
    : ICommandHandler<StartWorkOrderCommand>
{
    public async Task Handle(
        StartWorkOrderCommand request,
        CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new KnownException("工单不存在");

        workOrder.Start();
        logger.LogInformation("工单已启动，ID: {WorkOrderId}", request.Id);
    }
}

