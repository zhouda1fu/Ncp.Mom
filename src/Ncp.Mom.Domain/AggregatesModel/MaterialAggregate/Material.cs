using Ncp.Mom.Domain.DomainEvents;

namespace Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;

public partial record MaterialId : IGuidStronglyTypedId;

/// <summary>
/// 物料聚合根
/// </summary>
public partial class Material : Entity<MaterialId>, IAggregateRoot
{
    protected Material() { }

    public Material(
        string materialCode,
        string materialName,
        string? specification = null,
        string? unit = null)
    {
        MaterialCode = materialCode;
        MaterialName = materialName;
        Specification = specification;
        Unit = unit ?? "个";
        CreatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new MaterialCreatedDomainEvent(this));
    }

    public string MaterialCode { get; private set; } = string.Empty;
    public string MaterialName { get; private set; } = string.Empty;
    public string? Specification { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public Deleted IsDeleted { get; private set; } = new Deleted(false);
    public DeletedTime DeletedAt { get; private set; } = new DeletedTime(DateTimeOffset.UtcNow);
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 更新物料信息
    /// </summary>
    public void UpdateInfo(
        string materialCode,
        string materialName,
        string? specification = null,
        string? unit = null)
    {
        MaterialCode = materialCode;
        MaterialName = materialName;
        Specification = specification;
        if (!string.IsNullOrEmpty(unit))
        {
            Unit = unit;
        }
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 软删除物料
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new KnownException("物料已经被删除");

        IsDeleted = true;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }
}

