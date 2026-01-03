using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Web.Application.Queries;

/// <summary>
/// 工作中心查询DTO
/// </summary>
public record WorkCenterQueryDto(WorkCenterId Id, string WorkCenterCode, string WorkCenterName);

/// <summary>
/// 工作中心查询输入
/// </summary>
public class WorkCenterQueryInput : PageRequest
{
    public string? Keyword { get; set; }
}

/// <summary>
/// 工作中心查询服务
/// </summary>
public class WorkCenterQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<WorkCenter> WorkCenterSet { get; } = applicationDbContext.WorkCenters;

    /// <summary>
    /// 检查工作中心编码是否存在
    /// </summary>
    public async Task<bool> DoesWorkCenterExist(string workCenterCode, CancellationToken cancellationToken)
    {
        return await WorkCenterSet.AsNoTracking()
            .AnyAsync(w => w.WorkCenterCode == workCenterCode, cancellationToken);
    }

    /// <summary>
    /// 检查工作中心是否存在
    /// </summary>
    public async Task<bool> DoesWorkCenterExist(WorkCenterId workCenterId, CancellationToken cancellationToken)
    {
        return await WorkCenterSet.AsNoTracking()
            .AnyAsync(w => w.Id == workCenterId, cancellationToken);
    }

    /// <summary>
    /// 根据ID获取工作中心
    /// </summary>
    public async Task<WorkCenterQueryDto?> GetWorkCenterByIdAsync(WorkCenterId workCenterId, CancellationToken cancellationToken)
    {
        return await WorkCenterSet.AsNoTracking()
            .Where(w => w.Id == workCenterId)
            .Select(w => new WorkCenterQueryDto(w.Id, w.WorkCenterCode, w.WorkCenterName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取工作中心列表（分页）
    /// </summary>
    public async Task<PagedData<WorkCenterQueryDto>> GetWorkCentersAsync(WorkCenterQueryInput query, CancellationToken cancellationToken)
    {
        var queryable = WorkCenterSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(w => w.WorkCenterCode.Contains(query.Keyword) || w.WorkCenterName.Contains(query.Keyword));
        }

        return await queryable
            .OrderBy(w => w.WorkCenterCode)
            .Select(w => new WorkCenterQueryDto(w.Id, w.WorkCenterCode, w.WorkCenterName))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

