using Ncp.Mom.Domain.AggregatesModel.BomAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IBomRepository : IRepository<Bom, BomId> { }

public class BomRepository(ApplicationDbContext context) 
    : RepositoryBase<Bom, BomId, ApplicationDbContext>(context), 
      IBomRepository { }

