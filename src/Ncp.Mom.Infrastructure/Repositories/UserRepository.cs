using Ncp.Mom.Domain.AggregatesModel.UserAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User, UserId> { }

public class UserRepository(ApplicationDbContext context) : RepositoryBase<User, UserId, ApplicationDbContext>(context), IUserRepository { }

