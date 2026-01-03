using Ncp.Mom.Domain.AggregatesModel.OrderAggregate;
using Ncp.Mom.Domain.DomainEvents;

namespace Ncp.Mom.Domain.AggregatesModel.DeliverAggregate;

public partial record DeliverRecordId : IGuidStronglyTypedId;

/// <summary>
/// 交付记录聚合根
/// </summary>
public class DeliverRecord : Entity<DeliverRecordId>, IAggregateRoot
{
    protected DeliverRecord() { }

    public DeliverRecord(OrderId orderId)
    {
        OrderId = orderId;
        Status = DeliverStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new DeliverRecordCreatedDomainEvent(this));
    }

    public OrderId OrderId { get; private set; } = default!;
    public DeliverStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? StartTime { get; private set; }
    public DateTimeOffset? CompleteTime { get; private set; }
    public Deleted IsDeleted { get; private set; } = new Deleted(false);
    public DeletedTime DeletedAt { get; private set; } = new DeletedTime(DateTimeOffset.UtcNow);
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 开始交付
    /// </summary>
    public void StartDelivery()
    {
        if (Status != DeliverStatus.Pending)
            throw new KnownException("只能开始待交付状态的交付记录");

        Status = DeliverStatus.InTransit;
        StartTime = DateTimeOffset.UtcNow;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        AddDomainEvent(new DeliverRecordStartedDomainEvent(this));
    }

    /// <summary>
    /// 完成交付
    /// </summary>
    public void CompleteDelivery()
    {
        if (Status != DeliverStatus.InTransit)
            throw new KnownException("只能完成运输中的交付记录");

        Status = DeliverStatus.Delivered;
        CompleteTime = DateTimeOffset.UtcNow;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        AddDomainEvent(new DeliverRecordCompletedDomainEvent(this));
    }

    /// <summary>
    /// 取消交付
    /// </summary>
    public void CancelDelivery()
    {
        if (Status == DeliverStatus.Delivered)
            throw new KnownException("已完成的交付记录不能取消");

        Status = DeliverStatus.Cancelled;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        AddDomainEvent(new DeliverRecordCancelledDomainEvent(this));
    }

    /// <summary>
    /// 软删除交付记录
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new KnownException("交付记录已经被删除");

        IsDeleted = true;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }
}

/// <summary>
/// 交付状态
/// </summary>
public enum DeliverStatus
{
    Pending,    // 待交付
    InTransit,  // 运输中
    Delivered,  // 已交付
    Cancelled   // 已取消
}

