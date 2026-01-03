using Ncp.Mom.Domain.AggregatesModel.OrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.DeliverAggregate;

namespace Ncp.Mom.Domain.DomainEvents;

public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;

public record OrderPaidDomainEvent(Order Order) : IDomainEvent;

public record DeliverRecordCreatedDomainEvent(DeliverRecord DeliverRecord) : IDomainEvent;
public record DeliverRecordStartedDomainEvent(DeliverRecord DeliverRecord) : IDomainEvent;
public record DeliverRecordCompletedDomainEvent(DeliverRecord DeliverRecord) : IDomainEvent;
public record DeliverRecordCancelledDomainEvent(DeliverRecord DeliverRecord) : IDomainEvent;
