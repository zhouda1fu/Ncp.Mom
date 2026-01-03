using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.BomAggregate;

public partial record BomId : IGuidStronglyTypedId;
public partial record BomItemId : IGuidStronglyTypedId;

/// <summary>
/// BOM聚合根
/// </summary>
public partial class Bom : Entity<BomId>, IAggregateRoot
{
    protected Bom() { }

    public Bom(string bomNumber, ProductId productId, int version)
    {
        BomNumber = bomNumber;
        ProductId = productId;
        Version = version;
        IsActive = true;
        Items = new List<BomItem>();
        AddDomainEvent(new BomCreatedDomainEvent(this));
    }

    public string BomNumber { get; private set; } = string.Empty;
    public ProductId ProductId { get; private set; } = default!;
    public int Version { get; private set; }
    public bool IsActive { get; private set; }
    public List<BomItem> Items { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 添加BOM项
    /// </summary>
    public void AddItem(MaterialId materialId, decimal quantity, string unit)
    {
        if (Items.Any(i => i.MaterialId == materialId))
            throw new KnownException($"物料 {materialId} 已存在于BOM中");

        if (quantity <= 0)
            throw new KnownException("物料数量必须大于0");

        var item = new BomItem(materialId, quantity, unit);
        Items.Add(item);
    }

    /// <summary>
    /// 移除BOM项
    /// </summary>
    public void RemoveItem(BomItemId itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
        }
    }

    /// <summary>
    /// 停用BOM
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new KnownException("BOM已经停用");

        IsActive = false;
        AddDomainEvent(new BomDeactivatedDomainEvent(this));
    }

    /// <summary>
    /// 激活BOM
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            throw new KnownException("BOM已经激活");

        IsActive = true;
    }
}

/// <summary>
/// BOM项（子实体）
/// </summary>
public class BomItem : Entity<BomItemId>
{
    protected BomItem() { }

    public BomItem(MaterialId materialId, decimal quantity, string unit)
    {
        MaterialId = materialId;
        Quantity = quantity;
        Unit = unit;
    }

    public MaterialId MaterialId { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;

    /// <summary>
    /// 更新数量
    /// </summary>
    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new KnownException("物料数量必须大于0");

        Quantity = quantity;
    }
}

