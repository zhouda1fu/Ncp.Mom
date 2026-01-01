using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

public record ReportWorkOrderProgressCommand(
    WorkOrderId Id,
    int Quantity) : ICommand;

public class ReportWorkOrderProgressCommandValidator : AbstractValidator<ReportWorkOrderProgressCommand>
{
    public ReportWorkOrderProgressCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class ReportWorkOrderProgressCommandHandler(
    IWorkOrderRepository workOrderRepository,
    ILogger<ReportWorkOrderProgressCommandHandler> logger)
    : ICommandHandler<ReportWorkOrderProgressCommand>
{
    public async Task Handle(
        ReportWorkOrderProgressCommand request,
        CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new KnownException("工单不存在");

        workOrder.ReportProgress(request.Quantity);
        logger.LogInformation("工单 {WorkOrderId} 报工 {Quantity} 件，已完成 {CompletedQuantity}/{TotalQuantity}",
            request.Id, request.Quantity, workOrder.CompletedQuantity, workOrder.Quantity);
    }
}

