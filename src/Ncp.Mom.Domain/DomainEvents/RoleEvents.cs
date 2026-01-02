using Ncp.Mom.Domain.AggregatesModel.RoleAggregate;

namespace Ncp.Mom.Domain.DomainEvents.RoleEvents;

public record RoleInfoChangedDomainEvent(Role Role) : IDomainEvent;
public record RolePermissionChangedDomainEvent(Role Role) : IDomainEvent;

