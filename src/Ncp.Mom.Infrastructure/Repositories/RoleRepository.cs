using Ncp.Mom.Domain.AggregatesModel.RoleAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId> { }

public class RoleRepository(ApplicationDbContext context) : RepositoryBase<Role, RoleId, ApplicationDbContext>(context), IRoleRepository { }

