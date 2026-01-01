using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Web.Application.Queries.WorkOrders;

namespace Ncp.Mom.Web.Application.Queries;

public class WorkOrderQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<WorkOrder> WorkOrderSet { get; } = applicationDbContext.WorkOrders;

    public async Task<bool> DoesWorkOrderExist(string workOrderNumber, CancellationToken cancellationToken)
    {
        return await WorkOrderSet.AsNoTracking()
            .AnyAsync(w => w.WorkOrderNumber == workOrderNumber, cancellationToken);
    }

    public async Task<PagedData<WorkOrderDto>> GetWorkOrdersAsync(
        WorkOrderQueryInput query,
        CancellationToken cancellationToken)
    {
        var queryable = WorkOrderSet.AsNoTracking();

        if (query.ProductionPlanId != null)
        {
            queryable = queryable.Where(w => w.ProductionPlanId == query.ProductionPlanId);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(w => w.Status == query.Status.Value);
        }

        return await queryable
            .OrderByDescending(w => w.UpdateTime.Value)
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
            .ToPagedDataAsync(query, cancellationToken);
    }
}

