using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

/// <summary>
/// 质检单仓储接口
/// </summary>
public interface IQualityInspectionRepository : IRepository<QualityInspection, QualityInspectionId> { }

/// <summary>
/// 质检单仓储实现
/// </summary>
public class QualityInspectionRepository(ApplicationDbContext context) 
    : RepositoryBase<QualityInspection, QualityInspectionId, ApplicationDbContext>(context), 
      IQualityInspectionRepository { }

