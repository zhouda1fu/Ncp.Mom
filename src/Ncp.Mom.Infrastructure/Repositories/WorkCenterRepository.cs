using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

/// <summary>
/// 工作中心仓储接口
/// </summary>
public interface IWorkCenterRepository : IRepository<WorkCenter, WorkCenterId> { }

/// <summary>
/// 工作中心仓储实现
/// </summary>
public class WorkCenterRepository(ApplicationDbContext context) : RepositoryBase<WorkCenter, WorkCenterId, ApplicationDbContext>(context), IWorkCenterRepository { }

