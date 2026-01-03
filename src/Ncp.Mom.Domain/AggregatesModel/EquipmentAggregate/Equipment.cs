using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;

public partial record EquipmentId : IGuidStronglyTypedId;

/// <summary>
/// 设备聚合根
/// </summary>
public partial class Equipment : Entity<EquipmentId>, IAggregateRoot
{
    protected Equipment() { }

    public Equipment(
        string equipmentCode,
        string equipmentName,
        EquipmentType equipmentType,
        WorkCenterId? workCenterId)
    {
        EquipmentCode = equipmentCode;
        EquipmentName = equipmentName;
        EquipmentType = equipmentType;
        WorkCenterId = workCenterId;
        Status = EquipmentStatus.Idle;
        CurrentWorkOrderId = null;
        AddDomainEvent(new EquipmentCreatedDomainEvent(this));
    }

    public string EquipmentCode { get; private set; } = string.Empty;
    public string EquipmentName { get; private set; } = string.Empty;
    public EquipmentType EquipmentType { get; private set; }
    public WorkCenterId? WorkCenterId { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public WorkOrderId? CurrentWorkOrderId { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    /// <summary>
    /// 分配设备到工单
    /// </summary>
    public void AssignToWorkOrder(WorkOrderId workOrderId)
    {
        if (Status != EquipmentStatus.Idle)
            throw new KnownException("只能分配空闲状态的设备");

        CurrentWorkOrderId = workOrderId;
        Status = EquipmentStatus.Running;
        AddDomainEvent(new EquipmentAssignedDomainEvent(this, workOrderId));
    }

    /// <summary>
    /// 释放设备
    /// </summary>
    public void Release()
    {
        if (Status != EquipmentStatus.Running)
            throw new KnownException("只能释放运行状态的设备");

        CurrentWorkOrderId = null;
        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentReleasedDomainEvent(this));
    }

    /// <summary>
    /// 开始维护
    /// </summary>
    public void StartMaintenance()
    {
        if (Status == EquipmentStatus.Maintenance)
            throw new KnownException("设备已在维护中");

        if (Status == EquipmentStatus.Running)
            throw new KnownException("运行中的设备不能直接开始维护，请先释放设备");

        Status = EquipmentStatus.Maintenance;
        AddDomainEvent(new EquipmentMaintenanceStartedDomainEvent(this));
    }

    /// <summary>
    /// 完成维护
    /// </summary>
    public void CompleteMaintenance()
    {
        if (Status != EquipmentStatus.Maintenance)
            throw new KnownException("只能完成维护中的设备");

        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentMaintenanceCompletedDomainEvent(this));
    }

    /// <summary>
    /// 报告故障
    /// </summary>
    public void ReportFault()
    {
        if (Status == EquipmentStatus.Fault)
            throw new KnownException("设备已处于故障状态");

        Status = EquipmentStatus.Fault;
        AddDomainEvent(new EquipmentFaultReportedDomainEvent(this));
    }

    /// <summary>
    /// 修复故障
    /// </summary>
    public void RepairFault()
    {
        if (Status != EquipmentStatus.Fault)
            throw new KnownException("只能修复故障状态的设备");

        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentFaultRepairedDomainEvent(this));
    }

    /// <summary>
    /// 更新设备信息
    /// </summary>
    public void UpdateInfo(string equipmentCode, string equipmentName, WorkCenterId? workCenterId)
    {
        EquipmentCode = equipmentCode;
        EquipmentName = equipmentName;
        WorkCenterId = workCenterId;
    }
}

public enum EquipmentStatus
{
    Idle,        // 空闲
    Running,     // 运行中
    Maintenance, // 维护中
    Fault        // 故障
}

public enum EquipmentType
{
    Machine,     // 机器设备
    Tool,        // 工具
    Fixture      // 夹具
}

