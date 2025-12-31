using Ncp.Mom.Domain.AggregatesModel.OrderAggregate;

namespace Ncp.Mom.Domain.DomainEvents;

public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;

public record OrderPaidDomainEvent(Order Order) : IDomainEvent;
