using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;

public partial record ProductionPlanId : IGuidStronglyTypedId;

/// <summary>
/// 生产计划聚合根
/// </summary>
public partial class ProductionPlan : Entity<ProductionPlanId>, IAggregateRoot
{
    protected ProductionPlan() { }

    public ProductionPlan(
        string planNumber,
        ProductId productId,
        int quantity,
        DateTime plannedStartDate,
        DateTime plannedEndDate)
    {
        PlanNumber = planNumber;
        ProductId = productId;
        Quantity = quantity;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = ProductionPlanStatus.Draft;
        AddDomainEvent(new ProductionPlanCreatedDomainEvent(this));
    }

    public string PlanNumber { get; private set; } = string.Empty;
    public ProductId ProductId { get; private set; }=default!;
    public int Quantity { get; private set; }
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public ProductionPlanStatus Status { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void Approve()
    {
        if (Status != ProductionPlanStatus.Draft)
            throw new KnownException("只能审批草稿状态的生产计划");

        Status = ProductionPlanStatus.Approved;
        AddDomainEvent(new ProductionPlanApprovedDomainEvent(this));
    }

    public void Start()
    {
        if (Status != ProductionPlanStatus.Approved)
            throw new KnownException("只能启动已审批的生产计划");

        Status = ProductionPlanStatus.InProgress;
        AddDomainEvent(new ProductionPlanStartedDomainEvent(this));
    }

    public void Complete()
    {
        if (Status != ProductionPlanStatus.InProgress)
            throw new KnownException("只能完成进行中的生产计划");

        Status = ProductionPlanStatus.Completed;
        AddDomainEvent(new ProductionPlanCompletedDomainEvent(this));
    }

    public void Cancel()
    {
        if (Status == ProductionPlanStatus.Completed)
            throw new KnownException("已完成的生产计划不能取消");

        Status = ProductionPlanStatus.Cancelled;
        AddDomainEvent(new ProductionPlanCancelledDomainEvent(this));
    }
}

public enum ProductionPlanStatus
{
    Draft,      // 草稿
    Approved,   // 已审批
    InProgress, // 进行中
    Completed,  // 已完成
    Cancelled   // 已取消
}

