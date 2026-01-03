using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

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

public record QualityInspectionCreatedDomainEvent(QualityInspection QualityInspection) : IDomainEvent;
public record QualityInspectionCompletedDomainEvent(QualityInspection QualityInspection) : IDomainEvent;

public record EquipmentCreatedDomainEvent(Equipment Equipment) : IDomainEvent;
public record EquipmentAssignedDomainEvent(Equipment Equipment, WorkOrderId WorkOrderId) : IDomainEvent;
public record EquipmentReleasedDomainEvent(Equipment Equipment) : IDomainEvent;
public record EquipmentMaintenanceStartedDomainEvent(Equipment Equipment) : IDomainEvent;
public record EquipmentMaintenanceCompletedDomainEvent(Equipment Equipment) : IDomainEvent;
public record EquipmentFaultReportedDomainEvent(Equipment Equipment) : IDomainEvent;
public record EquipmentFaultRepairedDomainEvent(Equipment Equipment) : IDomainEvent;

public record BomCreatedDomainEvent(Bom Bom) : IDomainEvent;
public record BomDeactivatedDomainEvent(Bom Bom) : IDomainEvent;

public record MaterialCreatedDomainEvent(Material Material) : IDomainEvent;

public record ProductCreatedDomainEvent(Product Product) : IDomainEvent;

public record WorkCenterCreatedDomainEvent(WorkCenter WorkCenter) : IDomainEvent;

