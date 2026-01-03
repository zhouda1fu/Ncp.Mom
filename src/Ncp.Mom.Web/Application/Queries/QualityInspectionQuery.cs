using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure;

namespace Ncp.Mom.Web.Application.Queries;

/// <summary>
/// 质检单查询DTO
/// </summary>
public record QualityInspectionDto(
    QualityInspectionId Id,
    string InspectionNumber,
    WorkOrderId WorkOrderId,
    int SampleQuantity,
    int QualifiedQuantity,
    int UnqualifiedQuantity,
    QualityInspectionStatus Status,
    string? Remark,
    DateTimeOffset UpdateTime);

/// <summary>
/// 质检单查询输入
/// </summary>
public class QualityInspectionQueryInput : PageRequest
{
    public WorkOrderId? WorkOrderId { get; set; }
    public QualityInspectionStatus? Status { get; set; }
    public string? Keyword { get; set; }
}

/// <summary>
/// 质检单查询服务
/// </summary>
public class QualityInspectionQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<QualityInspection> QualityInspectionSet { get; } = applicationDbContext.Set<QualityInspection>();

    /// <summary>
    /// 根据ID获取质检单
    /// </summary>
    public async Task<QualityInspectionDto?> GetQualityInspectionByIdAsync(
        QualityInspectionId id, 
        CancellationToken cancellationToken)
    {
        return await QualityInspectionSet.AsNoTracking()
            .Where(q => q.Id == id)
            .Select(q => new QualityInspectionDto(
                q.Id,
                q.InspectionNumber,
                q.WorkOrderId,
                q.SampleQuantity,
                q.QualifiedQuantity,
                q.UnqualifiedQuantity,
                q.Status,
                q.Remark,
                q.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取质检单列表（分页）
    /// </summary>
    public async Task<PagedData<QualityInspectionDto>> GetQualityInspectionsAsync(
        QualityInspectionQueryInput query, 
        CancellationToken cancellationToken)
    {
        var queryable = QualityInspectionSet.AsNoTracking();

        if (query.WorkOrderId != null)
        {
            queryable = queryable.Where(q => q.WorkOrderId == query.WorkOrderId);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(q => q.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(q => q.InspectionNumber.Contains(query.Keyword));
        }

        return await queryable
            .OrderByDescending(q => q.Id)
            .Select(q => new QualityInspectionDto(
                q.Id,
                q.InspectionNumber,
                q.WorkOrderId,
                q.SampleQuantity,
                q.QualifiedQuantity,
                q.UnqualifiedQuantity,
                q.Status,
                q.Remark,
                q.UpdateTime.Value))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

