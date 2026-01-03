using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Infrastructure;

namespace Ncp.Mom.Web.Application.Queries;

public record MaterialDto(
    MaterialId Id,
    string MaterialCode,
    string MaterialName,
    string? Specification,
    string Unit,
    DateTimeOffset UpdateTime);

public class MaterialQueryInput : PageRequest
{
    public string? Keyword { get; set; }
}

public class MaterialQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Material> MaterialSet { get; } = applicationDbContext.Set<Material>();

    /// <summary>
    /// 检查物料是否存在
    /// </summary>
    public async Task<bool> DoesMaterialExist(MaterialId materialId, CancellationToken cancellationToken)
    {
        return await MaterialSet.AsNoTracking()
            .AnyAsync(m => m.Id == materialId, cancellationToken);
    }

    public async Task<MaterialDto?> GetMaterialByIdAsync(
        MaterialId id, 
        CancellationToken cancellationToken)
    {
        return await MaterialSet.AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MaterialDto(
                m.Id,
                m.MaterialCode,
                m.MaterialName,
                m.Specification,
                m.Unit,
                m.UpdateTime.Value))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedData<MaterialDto>> GetMaterialsAsync(
        MaterialQueryInput query, 
        CancellationToken cancellationToken)
    {
        var queryable = MaterialSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(m => 
                m.MaterialCode.Contains(query.Keyword) || 
                m.MaterialName.Contains(query.Keyword));
        }

        return await queryable
            .OrderBy(m => m.MaterialCode)
            .Select(m => new MaterialDto(
                m.Id,
                m.MaterialCode,
                m.MaterialName,
                m.Specification,
                m.Unit,
                m.UpdateTime.Value))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

