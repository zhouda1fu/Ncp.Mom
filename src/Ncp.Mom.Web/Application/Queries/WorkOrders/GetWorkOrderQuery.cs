using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

namespace Ncp.Mom.Web.Application.Queries.WorkOrders;

public record GetWorkOrderQuery(WorkOrderId Id) : IQuery<WorkOrderDto>;

public class GetWorkOrderQueryHandler(ApplicationDbContext applicationDbContext)
    : IQueryHandler<GetWorkOrderQuery, WorkOrderDto>
{
    public async Task<WorkOrderDto> Handle(
        GetWorkOrderQuery request,
        CancellationToken cancellationToken)
    {
        var result = await applicationDbContext.Set<WorkOrder>()
            .Where(w => w.Id == request.Id)
            .Select(w => new WorkOrderDto(
                w.Id,
                w.WorkOrderNumber,
                w.ProductionPlanId,
                w.ProductId,
                w.Quantity,
                w.CompletedQuantity,
                w.RoutingId,
                w.Status,
                w.StartTime,
                w.EndTime,
                w.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
        return result ?? throw new KnownException($"未找到工单，Id = {request.Id}");
    }
}

