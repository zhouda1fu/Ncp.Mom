using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

/// <summary>
/// 设备仓储接口
/// </summary>
public interface IEquipmentRepository : IRepository<Equipment, EquipmentId> { }

/// <summary>
/// 设备仓储实现
/// </summary>
public class EquipmentRepository(ApplicationDbContext context) 
    : RepositoryBase<Equipment, EquipmentId, ApplicationDbContext>(context), 
      IEquipmentRepository { }

