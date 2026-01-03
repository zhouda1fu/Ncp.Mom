using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;

public partial record QualityInspectionId : IGuidStronglyTypedId;

/// <summary>
/// 质检单聚合根
/// </summary>
public partial class QualityInspection : Entity<QualityInspectionId>, IAggregateRoot
{
    protected QualityInspection() { }

    public QualityInspection(
        string inspectionNumber,
        WorkOrderId workOrderId,
        int sampleQuantity)
    {
        InspectionNumber = inspectionNumber;
        WorkOrderId = workOrderId;
        SampleQuantity = sampleQuantity;
        QualifiedQuantity = 0;
        UnqualifiedQuantity = 0;
        Status = QualityInspectionStatus.Pending;
        AddDomainEvent(new QualityInspectionCreatedDomainEvent(this));
    }

    public string InspectionNumber { get; private set; } = string.Empty;
    public WorkOrderId WorkOrderId { get; private set; } = default!;
    public int SampleQuantity { get; private set; }
    public int QualifiedQuantity { get; private set; }
    public int UnqualifiedQuantity { get; private set; }
    public QualityInspectionStatus Status { get; private set; }
    public string? Remark { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 执行质检
    /// </summary>
    public void Inspect(int qualifiedQuantity, int unqualifiedQuantity, string? remark = null)
    {
        if (Status != QualityInspectionStatus.Pending)
            throw new KnownException("只能检验待检验状态的质检单");

        if (qualifiedQuantity < 0 || unqualifiedQuantity < 0)
            throw new KnownException("合格数量和不合格数量不能为负数");

        if (qualifiedQuantity + unqualifiedQuantity != SampleQuantity)
            throw new KnownException("合格数量与不合格数量之和必须等于抽样数量");

        QualifiedQuantity = qualifiedQuantity;
        UnqualifiedQuantity = unqualifiedQuantity;
        Remark = remark;
        Status = QualityInspectionStatus.Completed;
        AddDomainEvent(new QualityInspectionCompletedDomainEvent(this));
    }

    /// <summary>
    /// 开始检验
    /// </summary>
    public void StartInspection()
    {
        if (Status != QualityInspectionStatus.Pending)
            throw new KnownException("只能开始待检验状态的质检单");

        Status = QualityInspectionStatus.InProgress;
    }
}

public enum QualityInspectionStatus
{
    Pending,    // 待检验
    InProgress, // 检验中
    Completed   // 已完成
}

