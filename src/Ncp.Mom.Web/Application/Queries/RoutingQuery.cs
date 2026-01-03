using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Web.Application.Queries;

/// <summary>
/// 工艺路线查询DTO
/// </summary>
public record RoutingQueryDto(RoutingId Id, string RoutingNumber, string Name, ProductId ProductId, int OperationCount);

/// <summary>
/// 工艺路线查询输入
/// </summary>
public class RoutingQueryInput : PageRequest
{
    public string? Keyword { get; set; }
    public ProductId? ProductId { get; set; }
}

public class RoutingQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Routing> RoutingSet { get; } = applicationDbContext.Routings;

    public async Task<bool> DoesRoutingExist(string routingNumber, CancellationToken cancellationToken)
    {
        return await RoutingSet.AsNoTracking()
            .AnyAsync(r => r.RoutingNumber == routingNumber, cancellationToken);
    }

    /// <summary>
    /// 获取工艺路线列表（分页）
    /// </summary>
    public async Task<PagedData<RoutingQueryDto>> GetRoutingsAsync(RoutingQueryInput query, CancellationToken cancellationToken)
    {
        var queryable = RoutingSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(r => r.RoutingNumber.Contains(query.Keyword) || r.Name.Contains(query.Keyword));
        }

        if (query.ProductId != null)
        {
            queryable = queryable.Where(r => r.ProductId == query.ProductId);
        }

        return await queryable
            .OrderBy(r => r.RoutingNumber)
            .Select(r => new RoutingQueryDto(
                r.Id,
                r.RoutingNumber,
                r.Name,
                r.ProductId,
                r.Operations.Count))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

