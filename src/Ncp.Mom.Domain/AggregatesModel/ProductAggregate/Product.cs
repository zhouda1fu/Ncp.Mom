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
    }

    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// 更新产品信息
    /// </summary>
    public void UpdateInfo(string productCode, string productName)
    {
        ProductCode = productCode;
        ProductName = productName;
    }
}

