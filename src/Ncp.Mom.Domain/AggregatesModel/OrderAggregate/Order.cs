using Ncp.Mom.Domain.DomainEvents;
using NetCorePal.Extensions.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetCorePal.Extensions.Primitives;

namespace Ncp.Mom.Domain.AggregatesModel.OrderAggregate;

public partial record OrderId : IGuidStronglyTypedId;

/// <summary>
/// 聚合根
/// </summary>
public partial class Order : Entity<OrderId>, IAggregateRoot
{
    /// <summary>
    /// 受保护的默认构造函数，用以作为EF Core的反射入口
    /// </summary>
    protected Order()
    {
    }

    public Order(string name, int count)
    {
        this.Name = name;
        this.Count = count;
        this.CreatedAt = DateTimeOffset.UtcNow;
        this.AddDomainEvent(new OrderCreatedDomainEvent(this));
    }

    public bool Paid { get; private set; } = false;

    public string Name { get; private set; } = string.Empty;

    public int Count { get; private set; }

    public DateTimeOffset CreatedAt { get; init; }
    public Deleted IsDeleted { get; private set; } = new Deleted(false);
    public DeletedTime DeletedAt { get; private set; } = new DeletedTime(DateTimeOffset.UtcNow);
    public RowVersion RowVersion { get; private set; } = new RowVersion();

    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void OrderPaid()
    {
        if (Paid)
        {
            throw new KnownException("Order has been paid");
        }
        else
        {
            this.Paid = true;
            this.AddDomainEvent(new OrderPaidDomainEvent(this));
        }
    }

    /// <summary>
    /// 软删除订单
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new KnownException("订单已经被删除");

        IsDeleted = true;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }
}
