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
    }

    public string MaterialCode { get; private set; } = string.Empty;
    public string MaterialName { get; private set; } = string.Empty;
    public string? Specification { get; private set; }
    public string Unit { get; private set; } = string.Empty;
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
    }
}

