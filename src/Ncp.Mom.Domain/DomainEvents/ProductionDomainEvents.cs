using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

namespace Ncp.Mom.Domain.DomainEvents;

public record ProductionPlanCreatedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanApprovedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanStartedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanCompletedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanCancelledDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;

public record WorkOrderCreatedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderStartedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderPausedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderResumedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderProgressReportedDomainEvent(WorkOrder WorkOrder, int Quantity) : IDomainEvent;
public record WorkOrderCompletedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderCancelledDomainEvent(WorkOrder WorkOrder) : IDomainEvent;

public record RoutingCreatedDomainEvent(Routing Routing) : IDomainEvent;

