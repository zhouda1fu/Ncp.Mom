using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IMaterialRepository : IRepository<Material, MaterialId> { }

public class MaterialRepository(ApplicationDbContext context) 
    : RepositoryBase<Material, MaterialId, ApplicationDbContext>(context), 
      IMaterialRepository { }

