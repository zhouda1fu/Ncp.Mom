using Ncp.Mom.Domain.DomainEvents;

namespace Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

/// <summary>
/// 产品ID（基础类型，供其他聚合引用）
/// </summary>
public partial record ProductId : IGuidStronglyTypedId;

/// <summary>
/// 产品聚合根（简化版，主要用于引用）
/// </summary>
public partial class Product : Entity<ProductId>, IAggregateRoot
{
    protected Product() { }

    public Product(string productCode, string productName)
    {
        ProductCode = productCode;
        ProductName = productName;
        CreatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ProductCreatedDomainEvent(this));
    }

    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public Deleted IsDeleted { get; private set; } = new Deleted(false);
    public DeletedTime DeletedAt { get; private set; } = new DeletedTime(DateTimeOffset.UtcNow);
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 更新产品信息
    /// </summary>
    public void UpdateInfo(string productCode, string productName)
    {
        ProductCode = productCode;
        ProductName = productName;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 软删除产品
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new KnownException("产品已经被删除");

        IsDeleted = true;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }
}

