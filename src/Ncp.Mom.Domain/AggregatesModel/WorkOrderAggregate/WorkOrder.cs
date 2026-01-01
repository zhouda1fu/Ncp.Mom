using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

public partial record WorkOrderId : IGuidStronglyTypedId;

/// <summary>
/// 工单聚合根
/// </summary>
public partial class WorkOrder : Entity<WorkOrderId>, IAggregateRoot
{
    protected WorkOrder() { }

    public WorkOrder(
        string workOrderNumber,
        ProductionPlanId productionPlanId,
        ProductId productId,
        int quantity,
        RoutingId routingId)
    {
        WorkOrderNumber = workOrderNumber;
        ProductionPlanId = productionPlanId;
        ProductId = productId;
        Quantity = quantity;
        RoutingId = routingId;
        Status = WorkOrderStatus.Created;
        CompletedQuantity = 0;
        AddDomainEvent(new WorkOrderCreatedDomainEvent(this));
    }

    public string WorkOrderNumber { get; private set; } = string.Empty;
    public ProductionPlanId ProductionPlanId { get; private set; }= default!;
    public ProductId ProductId { get; private set; } = default!;
    public int Quantity { get; private set; }
    public int CompletedQuantity { get; private set; }
    public RoutingId RoutingId { get; private set; } = default!;
    public WorkOrderStatus Status { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void Start()
    {
        if (Status != WorkOrderStatus.Created && Status != WorkOrderStatus.Paused)
            throw new KnownException("只能启动已创建或已暂停的工单");

        Status = WorkOrderStatus.InProgress;
        StartTime = DateTimeOffset.UtcNow.DateTime;
        AddDomainEvent(new WorkOrderStartedDomainEvent(this));
    }

    public void Pause()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new KnownException("只能暂停进行中的工单");

        Status = WorkOrderStatus.Paused;
        AddDomainEvent(new WorkOrderPausedDomainEvent(this));
    }

    public void Resume()
    {
        if (Status != WorkOrderStatus.Paused)
            throw new KnownException("只能恢复已暂停的工单");

        Status = WorkOrderStatus.InProgress;
        AddDomainEvent(new WorkOrderResumedDomainEvent(this));
    }

    public void ReportProgress(int quantity)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new KnownException("只能报工进行中的工单");

        if (quantity <= 0)
            throw new KnownException("报工数量必须大于0");

        if (CompletedQuantity + quantity > Quantity)
            throw new KnownException("报工数量不能超过工单数量");

        CompletedQuantity += quantity;

        if (CompletedQuantity >= Quantity)
        {
            Status = WorkOrderStatus.Completed;
            EndTime = DateTimeOffset.UtcNow.DateTime;
            AddDomainEvent(new WorkOrderCompletedDomainEvent(this));
        }
        else
        {
            AddDomainEvent(new WorkOrderProgressReportedDomainEvent(this, quantity));
        }
    }

    public void Cancel()
    {
        if (Status == WorkOrderStatus.Completed)
            throw new KnownException("已完成的工单不能取消");

        Status = WorkOrderStatus.Cancelled;
        AddDomainEvent(new WorkOrderCancelledDomainEvent(this));
    }
}

public enum WorkOrderStatus
{
    Created,    // 已创建
    InProgress, // 进行中
    Paused,     // 已暂停
    Completed,  // 已完成
    Cancelled   // 已取消
}

