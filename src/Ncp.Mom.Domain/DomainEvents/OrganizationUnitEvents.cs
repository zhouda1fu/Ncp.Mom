using Ncp.Mom.Domain.AggregatesModel.OrganizationUnitAggregate;

namespace Ncp.Mom.Domain.DomainEvents.OrganizationUnitEvents;

/// <summary>
/// 组织架构信息变更领域事件
/// </summary>
public record OrganizationUnitInfoChangedDomainEvent(OrganizationUnit OrganizationUnit) : IDomainEvent;

