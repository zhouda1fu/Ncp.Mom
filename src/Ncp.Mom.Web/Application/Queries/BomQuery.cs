using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure;

namespace Ncp.Mom.Web.Application.Queries;

public record BomItemDto(
    BomItemId Id,
    MaterialId MaterialId,
    decimal Quantity,
    string Unit);

public record BomDto(
    BomId Id,
    string BomNumber,
    ProductId ProductId,
    int Version,
    bool IsActive,
    List<BomItemDto> Items,
    DateTimeOffset UpdateTime);

public class BomQueryInput : PageRequest
{
    public ProductId? ProductId { get; set; }
    public bool? IsActive { get; set; }
    public string? Keyword { get; set; }
}

public class BomQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Bom> BomSet { get; } = applicationDbContext.Set<Bom>();

    public async Task<BomDto?> GetBomByIdAsync(
        BomId id, 
        CancellationToken cancellationToken)
    {
        return await BomSet.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BomDto(
                b.Id,
                b.BomNumber,
                b.ProductId,
                b.Version,
                b.IsActive,
                b.Items.Select(i => new BomItemDto(
                    i.Id,
                    i.MaterialId,
                    i.Quantity,
                    i.Unit)).ToList(),
                b.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedData<BomDto>> GetBomsAsync(
        BomQueryInput query, 
        CancellationToken cancellationToken)
    {
        var queryable = BomSet.AsNoTracking();

        if (query.ProductId != null)
        {
            queryable = queryable.Where(b => b.ProductId == query.ProductId);
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(b => b.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(b => b.BomNumber.Contains(query.Keyword));
        }

        return await queryable
            .OrderByDescending(b => b.Id)
            .Select(b => new BomDto(
                b.Id,
                b.BomNumber,
                b.ProductId,
                b.Version,
                b.IsActive,
                b.Items.Select(i => new BomItemDto(
                    i.Id,
                    i.MaterialId,
                    i.Quantity,
                    i.Unit)).ToList(),
                b.UpdateTime.Value))
            .ToPagedDataAsync(query, cancellationToken);
    }

    public async Task<BomDto?> GetBomByProductAsync(
        ProductId productId, 
        CancellationToken cancellationToken)
    {
        return await BomSet.AsNoTracking()
            .Where(b => b.ProductId == productId && b.IsActive)
            .OrderByDescending(b => b.Version)
            .Select(b => new BomDto(
                b.Id,
                b.BomNumber,
                b.ProductId,
                b.Version,
                b.IsActive,
                b.Items.Select(i => new BomItemDto(
                    i.Id,
                    i.MaterialId,
                    i.Quantity,
                    i.Unit)).ToList(),
                b.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 检查物料是否被 BOM 使用
    /// </summary>
    public async Task<bool> IsMaterialUsedInBomAsync(
        MaterialId materialId,
        CancellationToken cancellationToken)
    {
        return await BomSet.AsNoTracking()
            .AnyAsync(b => b.Items.Any(i => i.MaterialId == materialId), cancellationToken);
    }
}

