using Ncp.Mom.Domain.DomainEvents;

namespace Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

/// <summary>
/// 工作中心ID（基础类型，供其他聚合引用）
/// </summary>
public partial record WorkCenterId : IGuidStronglyTypedId;

/// <summary>
/// 工作中心聚合根（简化版，主要用于引用）
/// </summary>
public partial class WorkCenter : Entity<WorkCenterId>, IAggregateRoot
{
    protected WorkCenter() { }

    public WorkCenter(string workCenterCode, string workCenterName)
    {
        WorkCenterCode = workCenterCode;
        WorkCenterName = workCenterName;
        CreatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new WorkCenterCreatedDomainEvent(this));
    }

    public string WorkCenterCode { get; private set; } = string.Empty;
    public string WorkCenterName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public Deleted IsDeleted { get; private set; } = new Deleted(false);
    public DeletedTime DeletedAt { get; private set; } = new DeletedTime(DateTimeOffset.UtcNow);
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 更新工作中心信息
    /// </summary>
    public void UpdateInfo(string workCenterCode, string workCenterName)
    {
        WorkCenterCode = workCenterCode;
        WorkCenterName = workCenterName;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 软删除工作中心
    /// </summary>
    public void SoftDelete()
    {
        if (IsDeleted)
            throw new KnownException("工作中心已经被删除");

        IsDeleted = true;
        UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
    }
}

