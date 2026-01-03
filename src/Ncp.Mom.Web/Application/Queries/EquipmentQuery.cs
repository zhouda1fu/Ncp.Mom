using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure;

namespace Ncp.Mom.Web.Application.Queries;

public record EquipmentDto(
    EquipmentId Id,
    string EquipmentCode,
    string EquipmentName,
    EquipmentType EquipmentType,
    WorkCenterId? WorkCenterId,
    EquipmentStatus Status,
    WorkOrderId? CurrentWorkOrderId,
    DateTimeOffset UpdateTime);

public class EquipmentQueryInput : PageRequest
{
    public EquipmentStatus? Status { get; set; }
    public EquipmentType? EquipmentType { get; set; }
    public WorkCenterId? WorkCenterId { get; set; }
    public string? Keyword { get; set; }
}

public class EquipmentQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Equipment> EquipmentSet { get; } = applicationDbContext.Set<Equipment>();

    public async Task<EquipmentDto?> GetEquipmentByIdAsync(
        EquipmentId id, 
        CancellationToken cancellationToken)
    {
        return await EquipmentSet.AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EquipmentDto(
                e.Id,
                e.EquipmentCode,
                e.EquipmentName,
                e.EquipmentType,
                e.WorkCenterId,
                e.Status,
                e.CurrentWorkOrderId,
                e.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedData<EquipmentDto>> GetEquipmentsAsync(
        EquipmentQueryInput query, 
        CancellationToken cancellationToken)
    {
        var queryable = EquipmentSet.AsNoTracking();

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(e => e.Status == query.Status.Value);
        }

        if (query.EquipmentType.HasValue)
        {
            queryable = queryable.Where(e => e.EquipmentType == query.EquipmentType.Value);
        }

        if (query.WorkCenterId != null)
        {
            queryable = queryable.Where(e => e.WorkCenterId == query.WorkCenterId);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(e => 
                e.EquipmentCode.Contains(query.Keyword) || 
                e.EquipmentName.Contains(query.Keyword));
        }

        return await queryable
            .OrderBy(e => e.EquipmentCode)
            .Select(e => new EquipmentDto(
                e.Id,
                e.EquipmentCode,
                e.EquipmentName,
                e.EquipmentType,
                e.WorkCenterId,
                e.Status,
                e.CurrentWorkOrderId,
                e.UpdateTime.Value))
            .ToPagedDataAsync(query, cancellationToken);
    }

    public async Task<List<EquipmentDto>> GetAvailableEquipmentsAsync(
        WorkCenterId? workCenterId,
        CancellationToken cancellationToken)
    {
        var queryable = EquipmentSet.AsNoTracking()
            .Where(e => e.Status == EquipmentStatus.Idle);

        if (workCenterId != null)
        {
            queryable = queryable.Where(e => e.WorkCenterId == workCenterId);
        }

        return await queryable
            .OrderBy(e => e.EquipmentCode)
            .Select(e => new EquipmentDto(
                e.Id,
                e.EquipmentCode,
                e.EquipmentName,
                e.EquipmentType,
                e.WorkCenterId,
                e.Status,
                e.CurrentWorkOrderId,
                e.UpdateTime.Value))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据工单ID获取设备ID列表
    /// </summary>
    public async Task<List<EquipmentId>> GetEquipmentIdsByWorkOrderIdAsync(
        WorkOrderId workOrderId,
        CancellationToken cancellationToken)
    {
        return await EquipmentSet.AsNoTracking()
            .Where(e => e.CurrentWorkOrderId == workOrderId)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);
    }
}

