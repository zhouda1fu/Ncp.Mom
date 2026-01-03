# 制造运营管理（MOM）系统实现方案

## 一、当前代码架构分析

### 1.1 架构模式

当前项目采用了**领域驱动设计（DDD）**和**CQRS（命令查询职责分离）**模式，具有以下特点：

#### 分层架构
```
┌─────────────────────────────────────┐
│   Ncp.Mom.Web (表现层)              │
│   - Endpoints (API端点)             │
│   - Commands (命令)                 │
│   - Queries (查询)                  │
│   - DomainEventHandlers             │
│   - IntegrationEventHandlers        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Ncp.Mom.Infrastructure (基础设施层)│
│   - ApplicationDbContext            │
│   - Repositories (仓储实现)         │
│   - EntityConfigurations            │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Ncp.Mom.Domain (领域层)            │
│   - AggregatesModel (聚合根)        │
│   - DomainEvents (领域事件)         │
└─────────────────────────────────────┘
```

#### 核心特性
- ✅ **强类型ID**：使用 `IGuidStronglyTypedId` 或 `IInt64StronglyTypedId`
- ✅ **聚合根模式**：每个聚合有独立的聚合根，实现 `IAggregateRoot`
- ✅ **领域事件**：聚合状态变更时发布领域事件
- ✅ **集成事件**：跨服务通信使用CAP框架
- ✅ **CQRS**：命令和查询分离
- ✅ **仓储模式**：通过仓储访问聚合数据
- ✅ **工作单元**：自动事务管理
- ✅ **乐观并发**：使用 `RowVersion` 控制

### 1.2 技术栈

- **框架**：ASP.NET Core + .NET 9
- **ORM**：Entity Framework Core
- **消息总线**：CAP (支持RabbitMQ/Kafka)
- **API框架**：FastEndpoints
- **命令处理**：MediatR
- **验证**：FluentValidation
- **数据库**：MySQL/PostgreSQL/SQL Server
- **缓存**：Redis
- **任务调度**：Hangfire
- **监控**：Prometheus

## 二、MOM系统核心业务域分析

### 2.1 MOM系统主要业务域

制造运营管理系统（MOM）通常包含以下核心业务域：

#### 1. 生产计划管理（Production Planning）
- 主生产计划（MPS）
- 物料需求计划（MRP）
- 生产排程

#### 2. 生产执行管理（Production Execution）
- 工单管理（Work Order）
- 工艺路线（Routing）
- 生产报工（Production Reporting）
- 生产进度跟踪

#### 3. 质量管理（Quality Management）
- 质检计划
- 质检执行
- 不合格品处理
- 质量追溯

#### 4. 设备管理（Equipment Management）
- 设备台账
- 设备维护计划
- 设备状态监控
- 故障管理

#### 5. 物料管理（Material Management）
- 物料清单（BOM）
- 物料需求
- 物料消耗
- 库存管理

#### 6. 工艺管理（Process Management）
- 工艺路线定义
- 工序定义
- 工艺参数

#### 7. 数据采集（Data Collection）
- 生产数据采集
- 设备数据采集
- 质量数据采集

## 三、基于当前框架的MOM系统设计

### 3.1 聚合根设计

#### 3.1.1 生产计划聚合（ProductionPlan）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/ProductionPlanAggregate/ProductionPlan.cs

public partial record ProductionPlanId : IGuidStronglyTypedId;

public partial class ProductionPlan : Entity<ProductionPlanId>, IAggregateRoot
{
    protected ProductionPlan() { }

    public ProductionPlan(
        string planNumber,
        ProductId productId,
        int quantity,
        DateTime plannedStartDate,
        DateTime plannedEndDate)
    {
        PlanNumber = planNumber;
        ProductId = productId;
        Quantity = quantity;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = ProductionPlanStatus.Draft;
        AddDomainEvent(new ProductionPlanCreatedDomainEvent(this));
    }

    public string PlanNumber { get; private set; } = string.Empty;
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public ProductionPlanStatus Status { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void Approve()
    {
        if (Status != ProductionPlanStatus.Draft)
            throw new KnownException("只能审批草稿状态的生产计划");
        
        Status = ProductionPlanStatus.Approved;
        AddDomainEvent(new ProductionPlanApprovedDomainEvent(this));
    }

    public void Start()
    {
        if (Status != ProductionPlanStatus.Approved)
            throw new KnownException("只能启动已审批的生产计划");
        
        Status = ProductionPlanStatus.InProgress;
        AddDomainEvent(new ProductionPlanStartedDomainEvent(this));
    }

    public void Complete()
    {
        if (Status != ProductionPlanStatus.InProgress)
            throw new KnownException("只能完成进行中的生产计划");
        
        Status = ProductionPlanStatus.Completed;
        AddDomainEvent(new ProductionPlanCompletedDomainEvent(this));
    }
}

public enum ProductionPlanStatus
{
    Draft,      // 草稿
    Approved,   // 已审批
    InProgress, // 进行中
    Completed,  // 已完成
    Cancelled   // 已取消
}
```

#### 3.1.2 工单聚合（WorkOrder）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/WorkOrderAggregate/WorkOrder.cs

public partial record WorkOrderId : IGuidStronglyTypedId;

public partial class WorkOrder : Entity<WorkOrderId>, IAggregateRoot
{
    protected WorkOrder() { }

    public WorkOrder(
        string workOrderNumber,
        ProductionPlanId productionPlanId,
        ProductId productId,
        int quantity,
        RoutingId routingId)
    {
        WorkOrderNumber = workOrderNumber;
        ProductionPlanId = productionPlanId;
        ProductId = productId;
        Quantity = quantity;
        RoutingId = routingId;
        Status = WorkOrderStatus.Created;
        AddDomainEvent(new WorkOrderCreatedDomainEvent(this));
    }

    public string WorkOrderNumber { get; private set; } = string.Empty;
    public ProductionPlanId ProductionPlanId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int CompletedQuantity { get; private set; }
    public RoutingId RoutingId { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void Start()
    {
        if (Status != WorkOrderStatus.Created && Status != WorkOrderStatus.Paused)
            throw new KnownException("只能启动已创建或已暂停的工单");
        
        Status = WorkOrderStatus.InProgress;
        StartTime = DateTimeOffset.UtcNow.DateTime;
        AddDomainEvent(new WorkOrderStartedDomainEvent(this));
    }

    public void ReportProgress(int quantity)
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new KnownException("只能报工进行中的工单");
        
        if (CompletedQuantity + quantity > Quantity)
            throw new KnownException("报工数量不能超过工单数量");
        
        CompletedQuantity += quantity;
        
        if (CompletedQuantity >= Quantity)
        {
            Status = WorkOrderStatus.Completed;
            EndTime = DateTimeOffset.UtcNow.DateTime;
            AddDomainEvent(new WorkOrderCompletedDomainEvent(this));
        }
        else
        {
            AddDomainEvent(new WorkOrderProgressReportedDomainEvent(this, quantity));
        }
    }
}

public enum WorkOrderStatus
{
    Created,    // 已创建
    InProgress, // 进行中
    Paused,     // 已暂停
    Completed,  // 已完成
    Cancelled   // 已取消
}
```

#### 3.1.3 工艺路线聚合（Routing）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/RoutingAggregate/Routing.cs

public partial record RoutingId : IGuidStronglyTypedId;

public partial class Routing : Entity<RoutingId>, IAggregateRoot
{
    protected Routing() { }

    public Routing(string routingNumber, string name, ProductId productId)
    {
        RoutingNumber = routingNumber;
        Name = name;
        ProductId = productId;
        Operations = new List<RoutingOperation>();
        AddDomainEvent(new RoutingCreatedDomainEvent(this));
    }

    public string RoutingNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ProductId ProductId { get; private set; }
    public List<RoutingOperation> Operations { get; private set; } = new();

    public void AddOperation(int sequence, string operationName, WorkCenterId workCenterId, decimal standardTime)
    {
        if (Operations.Any(o => o.Sequence == sequence))
            throw new KnownException($"工序序号 {sequence} 已存在");

        var operation = new RoutingOperation(sequence, operationName, workCenterId, standardTime);
        Operations.Add(operation);
    }
}

// 子实体：工艺路线工序
public class RoutingOperation : Entity<RoutingOperationId>
{
    protected RoutingOperation() { }

    public RoutingOperation(int sequence, string operationName, WorkCenterId workCenterId, decimal standardTime)
    {
        Sequence = sequence;
        OperationName = operationName;
        WorkCenterId = workCenterId;
        StandardTime = standardTime;
    }

    public int Sequence { get; private set; }
    public string OperationName { get; private set; } = string.Empty;
    public WorkCenterId WorkCenterId { get; private set; }
    public decimal StandardTime { get; private set; } // 标准工时（小时）
}

public partial record RoutingOperationId : IGuidStronglyTypedId;
```

#### 3.1.4 质检单聚合（QualityInspection）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/QualityInspectionAggregate/QualityInspection.cs

public partial record QualityInspectionId : IGuidStronglyTypedId;

public partial class QualityInspection : Entity<QualityInspectionId>, IAggregateRoot
{
    protected QualityInspection() { }

    public QualityInspection(
        string inspectionNumber,
        WorkOrderId workOrderId,
        int sampleQuantity)
    {
        InspectionNumber = inspectionNumber;
        WorkOrderId = workOrderId;
        SampleQuantity = sampleQuantity;
        Status = QualityInspectionStatus.Pending;
        AddDomainEvent(new QualityInspectionCreatedDomainEvent(this));
    }

    public string InspectionNumber { get; private set; } = string.Empty;
    public WorkOrderId WorkOrderId { get; private set; }
    public int SampleQuantity { get; private set; }
    public int QualifiedQuantity { get; private set; }
    public int UnqualifiedQuantity { get; private set; }
    public QualityInspectionStatus Status { get; private set; }
    public string? Remark { get; private set; }

    public void Inspect(int qualifiedQuantity, int unqualifiedQuantity, string? remark = null)
    {
        if (Status != QualityInspectionStatus.Pending)
            throw new KnownException("只能检验待检验状态的质检单");
        
        if (qualifiedQuantity + unqualifiedQuantity != SampleQuantity)
            throw new KnownException("合格数量与不合格数量之和必须等于抽样数量");

        QualifiedQuantity = qualifiedQuantity;
        UnqualifiedQuantity = unqualifiedQuantity;
        Remark = remark;
        Status = QualityInspectionStatus.Completed;
        AddDomainEvent(new QualityInspectionCompletedDomainEvent(this));
    }
}

public enum QualityInspectionStatus
{
    Pending,    // 待检验
    InProgress, // 检验中
    Completed   // 已完成
}
```

#### 3.1.5 设备聚合（Equipment）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/EquipmentAggregate/Equipment.cs

public partial record EquipmentId : IGuidStronglyTypedId;

public partial class Equipment : Entity<EquipmentId>, IAggregateRoot
{
    protected Equipment() { }

    public Equipment(
        string equipmentCode,
        string equipmentName,
        EquipmentType equipmentType,
        WorkCenterId? workCenterId)
    {
        EquipmentCode = equipmentCode;
        EquipmentName = equipmentName;
        EquipmentType = equipmentType;
        WorkCenterId = workCenterId;
        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentCreatedDomainEvent(this));
    }

    public string EquipmentCode { get; private set; } = string.Empty;
    public string EquipmentName { get; private set; } = string.Empty;
    public EquipmentType EquipmentType { get; private set; }
    public WorkCenterId? WorkCenterId { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public WorkOrderId? CurrentWorkOrderId { get; private set; }

    public void AssignToWorkOrder(WorkOrderId workOrderId)
    {
        if (Status != EquipmentStatus.Idle)
            throw new KnownException("只能分配空闲状态的设备");

        CurrentWorkOrderId = workOrderId;
        Status = EquipmentStatus.Running;
        AddDomainEvent(new EquipmentAssignedDomainEvent(this, workOrderId));
    }

    public void Release()
    {
        if (Status != EquipmentStatus.Running)
            throw new KnownException("只能释放运行状态的设备");

        CurrentWorkOrderId = null;
        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentReleasedDomainEvent(this));
    }

    public void StartMaintenance()
    {
        if (Status == EquipmentStatus.Maintenance)
            throw new KnownException("设备已在维护中");

        Status = EquipmentStatus.Maintenance;
        AddDomainEvent(new EquipmentMaintenanceStartedDomainEvent(this));
    }

    public void CompleteMaintenance()
    {
        if (Status != EquipmentStatus.Maintenance)
            throw new KnownException("只能完成维护中的设备");

        Status = EquipmentStatus.Idle;
        AddDomainEvent(new EquipmentMaintenanceCompletedDomainEvent(this));
    }
}

public enum EquipmentStatus
{
    Idle,        // 空闲
    Running,     // 运行中
    Maintenance, // 维护中
    Fault        // 故障
}

public enum EquipmentType
{
    Machine,     // 机器设备
    Tool,        // 工具
    Fixture      // 夹具
}
```

#### 3.1.6 物料清单聚合（BOM - Bill of Materials）

```csharp
// 位置：src/Ncp.Mom.Domain/AggregatesModel/BomAggregate/Bom.cs

public partial record BomId : IGuidStronglyTypedId;

public partial class Bom : Entity<BomId>, IAggregateRoot
{
    protected Bom() { }

    public Bom(string bomNumber, ProductId productId, int version)
    {
        BomNumber = bomNumber;
        ProductId = productId;
        Version = version;
        Items = new List<BomItem>();
        AddDomainEvent(new BomCreatedDomainEvent(this));
    }

    public string BomNumber { get; private set; } = string.Empty;
    public ProductId ProductId { get; private set; }
    public int Version { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<BomItem> Items { get; private set; } = new();

    public void AddItem(MaterialId materialId, decimal quantity, string unit)
    {
        if (Items.Any(i => i.MaterialId == materialId))
            throw new KnownException($"物料 {materialId} 已存在于BOM中");

        var item = new BomItem(materialId, quantity, unit);
        Items.Add(item);
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new BomDeactivatedDomainEvent(this));
    }
}

// 子实体：BOM项
public class BomItem : Entity<BomItemId>
{
    protected BomItem() { }

    public BomItem(MaterialId materialId, decimal quantity, string unit)
    {
        MaterialId = materialId;
        Quantity = quantity;
        Unit = unit;
    }

    public MaterialId MaterialId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
}

public partial record BomItemId : IGuidStronglyTypedId;
```

### 3.2 领域事件设计

```csharp
// 位置：src/Ncp.Mom.Domain/DomainEvents/ProductionDomainEvents.cs

namespace Ncp.Mom.Domain.DomainEvents;

public record ProductionPlanCreatedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanApprovedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanStartedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;
public record ProductionPlanCompletedDomainEvent(ProductionPlan ProductionPlan) : IDomainEvent;

public record WorkOrderCreatedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderStartedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;
public record WorkOrderProgressReportedDomainEvent(WorkOrder WorkOrder, int Quantity) : IDomainEvent;
public record WorkOrderCompletedDomainEvent(WorkOrder WorkOrder) : IDomainEvent;

public record QualityInspectionCreatedDomainEvent(QualityInspection QualityInspection) : IDomainEvent;
public record QualityInspectionCompletedDomainEvent(QualityInspection QualityInspection) : IDomainEvent;

public record EquipmentAssignedDomainEvent(Equipment Equipment, WorkOrderId WorkOrderId) : IDomainEvent;
public record EquipmentReleasedDomainEvent(Equipment Equipment) : IDomainEvent;
```

### 3.3 命令设计示例

#### 3.3.1 创建生产计划命令

```csharp
// 位置：src/Ncp.Mom.Web/Application/Commands/ProductionPlans/CreateProductionPlanCommand.cs

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

public record CreateProductionPlanCommand(
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate) : ICommand<ProductionPlanId>;

public class CreateProductionPlanCommandValidator : AbstractValidator<CreateProductionPlanCommand>
{
    public CreateProductionPlanCommandValidator()
    {
        RuleFor(x => x.PlanNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PlannedEndDate).GreaterThan(x => x.PlannedStartDate);
    }
}

public class CreateProductionPlanCommandHandler(
    IProductionPlanRepository productionPlanRepository,
    ILogger<CreateProductionPlanCommandHandler> logger) 
    : ICommandHandler<CreateProductionPlanCommand, ProductionPlanId>
{
    public async Task<ProductionPlanId> Handle(
        CreateProductionPlanCommand request, 
        CancellationToken cancellationToken)
    {
        var plan = new ProductionPlan(
            request.PlanNumber,
            request.ProductId,
            request.Quantity,
            request.PlannedStartDate,
            request.PlannedEndDate);

        plan = await productionPlanRepository.AddAsync(plan, cancellationToken);
        logger.LogInformation("生产计划已创建，ID: {ProductionPlanId}", plan.Id);
        return plan.Id;
    }
}
```

#### 3.3.2 创建工单命令

```csharp
// 位置：src/Ncp.Mom.Web/Application/Commands/WorkOrders/CreateWorkOrderCommand.cs

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

public record CreateWorkOrderCommand(
    string WorkOrderNumber,
    ProductionPlanId ProductionPlanId,
    ProductId ProductId,
    int Quantity,
    RoutingId RoutingId) : ICommand<WorkOrderId>;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class CreateWorkOrderCommandHandler(
    IWorkOrderRepository workOrderRepository,
    ILogger<CreateWorkOrderCommandHandler> logger) 
    : ICommandHandler<CreateWorkOrderCommand, WorkOrderId>
{
    public async Task<WorkOrderId> Handle(
        CreateWorkOrderCommand request, 
        CancellationToken cancellationToken)
    {
        var workOrder = new WorkOrder(
            request.WorkOrderNumber,
            request.ProductionPlanId,
            request.ProductId,
            request.Quantity,
            request.RoutingId);

        workOrder = await workOrderRepository.AddAsync(workOrder, cancellationToken);
        logger.LogInformation("工单已创建，ID: {WorkOrderId}", workOrder.Id);
        return workOrder.Id;
    }
}
```

#### 3.3.3 工单报工命令

```csharp
// 位置：src/Ncp.Mom.Web/Application/Commands/WorkOrders/ReportWorkOrderProgressCommand.cs

namespace Ncp.Mom.Web.Application.Commands.WorkOrders;

public record ReportWorkOrderProgressCommand(
    WorkOrderId WorkOrderId,
    int Quantity) : ICommand;

public class ReportWorkOrderProgressCommandValidator : AbstractValidator<ReportWorkOrderProgressCommand>
{
    public ReportWorkOrderProgressCommandValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class ReportWorkOrderProgressCommandHandler(
    IWorkOrderRepository workOrderRepository,
    ILogger<ReportWorkOrderProgressCommandHandler> logger) 
    : ICommandHandler<ReportWorkOrderProgressCommand>
{
    public async Task Handle(
        ReportWorkOrderProgressCommand request, 
        CancellationToken cancellationToken)
    {
        var workOrder = await workOrderRepository.GetAsync(request.WorkOrderId, cancellationToken)
            ?? throw new KnownException("工单不存在");

        workOrder.ReportProgress(request.Quantity);
        logger.LogInformation("工单 {WorkOrderId} 报工 {Quantity} 件", request.WorkOrderId, request.Quantity);
    }
}
```

### 3.4 领域事件处理器示例

```csharp
// 位置：src/Ncp.Mom.Web/Application/DomainEventHandlers/WorkOrderCreatedDomainEventHandler.cs

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

public class WorkOrderCreatedDomainEventHandler(
    IMediator mediator,
    ILogger<WorkOrderCreatedDomainEventHandler> logger) 
    : IDomainEventHandler<WorkOrderCreatedDomainEvent>
{
    public async Task Handle(
        WorkOrderCreatedDomainEvent notification, 
        CancellationToken cancellationToken)
    {
        var workOrder = notification.WorkOrder;
        
        // 自动创建质检单
        var createInspectionCommand = new CreateQualityInspectionCommand(
            $"QC-{workOrder.WorkOrderNumber}",
            workOrder.Id,
            CalculateSampleQuantity(workOrder.Quantity));

        await mediator.Send(createInspectionCommand, cancellationToken);
        
        logger.LogInformation("工单 {WorkOrderId} 创建后自动创建质检单", workOrder.Id);
    }

    private int CalculateSampleQuantity(int totalQuantity)
    {
        // 根据抽样规则计算抽样数量
        return totalQuantity >= 100 ? (int)(totalQuantity * 0.1) : totalQuantity;
    }
}
```

### 3.5 API端点设计示例

```csharp
// 位置：src/Ncp.Mom.Web/Endpoints/ProductionPlanEndpoints/CreateProductionPlanEndpoint.cs

namespace Ncp.Mom.Web.Endpoints.ProductionPlanEndpoints;

public record CreateProductionPlanRequest(
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate);

[Tags("ProductionPlans")]
[HttpPost("/api/production-plans")]
[Authorize]
public class CreateProductionPlanEndpoint(IMediator mediator) 
    : Endpoint<CreateProductionPlanRequest, ResponseData<ProductionPlanId>>
{
    public override async Task HandleAsync(
        CreateProductionPlanRequest req, 
        CancellationToken ct)
    {
        var cmd = new CreateProductionPlanCommand(
            req.PlanNumber,
            req.ProductId,
            req.Quantity,
            req.PlannedStartDate,
            req.PlannedEndDate);

        var id = await mediator.Send(cmd, ct);
        await Send.OkAsync(id.AsResponseData(), cancellation: ct);
    }
}
```

### 3.6 仓储接口和实现

```csharp
// 位置：src/Ncp.Mom.Infrastructure/Repositories/ProductionPlanRepository.cs

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IProductionPlanRepository : IRepository<ProductionPlan, ProductionPlanId>
{
    Task<ProductionPlan?> GetByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default);
}

public class ProductionPlanRepository(ApplicationDbContext context) 
    : RepositoryBase<ProductionPlan, ProductionPlanId, ApplicationDbContext>(context), 
      IProductionPlanRepository
{
    public async Task<ProductionPlan?> GetByPlanNumberAsync(
        string planNumber, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<ProductionPlan>()
            .FirstOrDefaultAsync(p => p.PlanNumber == planNumber, cancellationToken);
    }
}
```

### 3.7 实体配置示例

```csharp
// 位置：src/Ncp.Mom.Infrastructure/EntityConfigurations/ProductionPlanEntityTypeConfiguration.cs

namespace Ncp.Mom.Infrastructure.EntityConfigurations;

internal class ProductionPlanEntityTypeConfiguration : IEntityTypeConfiguration<ProductionPlan>
{
    public void Configure(EntityTypeBuilder<ProductionPlan> builder)
    {
        builder.ToTable("production_plan");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseGuidVersion7ValueGenerator();
        builder.Property(b => b.PlanNumber).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Quantity).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.PlannedStartDate).IsRequired();
        builder.Property(b => b.PlannedEndDate).IsRequired();
        
        builder.HasIndex(b => b.PlanNumber).IsUnique();
    }
}
```

## 四、MOM系统核心业务流程

### 4.1 生产计划到工单流程

```
1. 创建生产计划 (CreateProductionPlanCommand)
   └─> 发布 ProductionPlanCreatedDomainEvent

2. 审批生产计划 (ApproveProductionPlanCommand)
   └─> 发布 ProductionPlanApprovedDomainEvent

3. 启动生产计划 (StartProductionPlanCommand)
   └─> 发布 ProductionPlanStartedDomainEvent
   └─> 领域事件处理器自动创建工单

4. 工单创建 (CreateWorkOrderCommand)
   └─> 发布 WorkOrderCreatedDomainEvent
   └─> 领域事件处理器自动创建质检单

5. 工单报工 (ReportWorkOrderProgressCommand)
   └─> 发布 WorkOrderProgressReportedDomainEvent
   └─> 工单完成时发布 WorkOrderCompletedDomainEvent
```

### 4.2 质量检验流程

```
1. 工单创建时自动创建质检单
   └─> QualityInspectionCreatedDomainEvent

2. 执行质检 (InspectQualityCommand)
   └─> QualityInspectionCompletedDomainEvent
   └─> 如果不合格，触发不合格品处理流程
```

### 4.3 设备分配流程

```
1. 工单启动时分配设备 (AssignEquipmentCommand)
   └─> EquipmentAssignedDomainEvent

2. 工单完成时释放设备 (ReleaseEquipmentCommand)
   └─> EquipmentReleasedDomainEvent
```

## 五、集成事件设计（跨服务通信）

### 5.1 集成事件示例

```csharp
// 位置：src/Ncp.Mom.Web/Application/IntegrationEvents/WorkOrderCompletedIntegrationEvent.cs

namespace Ncp.Mom.Web.Application.IntegrationEvents;

public record WorkOrderCompletedIntegrationEvent(
    WorkOrderId WorkOrderId,
    ProductId ProductId,
    int Quantity,
    DateTime CompletedTime) : IntegrationEvent;
```

### 5.2 集成事件转换器

```csharp
// 位置：src/Ncp.Mom.Web/Application/IntegrationEventConverters/WorkOrderCompletedIntegrationEventConverter.cs

namespace Ncp.Mom.Web.Application.IntegrationEventConverters;

public class WorkOrderCompletedIntegrationEventConverter 
    : IIntegrationEventConverter<WorkOrderCompletedDomainEvent, WorkOrderCompletedIntegrationEvent>
{
    public WorkOrderCompletedIntegrationEvent Convert(
        WorkOrderCompletedDomainEvent domainEvent)
    {
        return new WorkOrderCompletedIntegrationEvent(
            domainEvent.WorkOrder.Id,
            domainEvent.WorkOrder.ProductId,
            domainEvent.WorkOrder.CompletedQuantity,
            domainEvent.WorkOrder.EndTime!.Value);
    }
}
```

## 六、查询设计（CQRS查询端）

### 6.1 查询示例

```csharp
// 位置：src/Ncp.Mom.Web/Application/Queries/ProductionPlans/GetProductionPlanQuery.cs

namespace Ncp.Mom.Web.Application.Queries.ProductionPlans;

public record GetProductionPlanQuery(ProductionPlanId Id) : IRequest<ProductionPlanDto?>;

public record ProductionPlanDto(
    ProductionPlanId Id,
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    ProductionPlanStatus Status,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate);

public class GetProductionPlanQueryHandler(
    ApplicationDbContext context) 
    : IRequestHandler<GetProductionPlanQuery, ProductionPlanDto?>
{
    public async Task<ProductionPlanDto?> Handle(
        GetProductionPlanQuery request, 
        CancellationToken cancellationToken)
    {
        return await context.Set<ProductionPlan>()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductionPlanDto(
                p.Id,
                p.PlanNumber,
                p.ProductId,
                p.Quantity,
                p.Status,
                p.PlannedStartDate,
                p.PlannedEndDate))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

## 七、实施步骤建议

### 阶段一：核心域建模（1-2周）
1. ✅ 定义核心聚合根（ProductionPlan, WorkOrder, Routing）
2. ✅ 定义领域事件
3. ✅ 创建仓储接口和实现
4. ✅ 配置实体映射

### 阶段二：基础功能实现（2-3周）
1. ✅ 实现生产计划管理（创建、审批、启动）
2. ✅ 实现工单管理（创建、启动、报工）
3. ✅ 实现工艺路线管理
4. ✅ 实现基础API端点

### 阶段三：扩展功能（2-3周）
1. ✅ 实现质量管理（质检单）
2. ✅ 实现设备管理
3. ✅ 实现物料清单（BOM）
4. ✅ 实现数据采集接口

### 阶段四：集成和优化（1-2周）
1. ✅ 实现集成事件（跨服务通信）
2. ✅ 实现查询端（CQRS）
3. ✅ 性能优化
4. ✅ 单元测试

## 八、关键技术点

### 8.1 事务管理
- 框架自动处理工作单元，命令处理器无需手动调用 `SaveChanges`
- 领域事件在同一事务中发布和处理

### 8.2 并发控制
- 使用 `RowVersion` 实现乐观并发控制
- 使用 Redis 分布式锁处理并发命令

### 8.3 事件驱动
- 领域事件：聚合内状态变更
- 集成事件：跨服务通信（使用CAP框架）

### 8.4 查询优化
- CQRS模式分离读写
- 查询端可直接使用EF Core查询，无需通过仓储

## 九、总结

当前框架非常适合实现MOM系统，因为：

1. **DDD架构**：清晰的分层和聚合设计，符合制造业复杂业务域
2. **CQRS模式**：生产数据查询频繁，读写分离提升性能
3. **事件驱动**：生产流程复杂，事件驱动解耦业务逻辑
4. **强类型ID**：提升代码可读性和类型安全
5. **自动事务**：简化开发，减少错误
6. **集成事件**：支持微服务架构，便于系统扩展

按照上述设计，可以逐步构建完整的MOM系统，同时保持代码的可维护性和可扩展性。

