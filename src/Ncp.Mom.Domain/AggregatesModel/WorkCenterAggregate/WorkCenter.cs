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
    }

    public string WorkCenterCode { get; private set; } = string.Empty;
    public string WorkCenterName { get; private set; } = string.Empty;

    /// <summary>
    /// 更新工作中心信息
    /// </summary>
    public void UpdateInfo(string workCenterCode, string workCenterName)
    {
        WorkCenterCode = workCenterCode;
        WorkCenterName = workCenterName;
    }
}

