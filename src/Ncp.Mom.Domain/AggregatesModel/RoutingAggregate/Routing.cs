using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

public partial record RoutingId : IGuidStronglyTypedId;

/// <summary>
/// 工艺路线聚合根
/// </summary>
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
    public ProductId ProductId { get; private set; }= default!;
    public List<RoutingOperation> Operations { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new RowVersion();
    public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

    public void AddOperation(int sequence, string operationName, WorkCenterId workCenterId, decimal standardTime)
    {
        if (Operations.Any(o => o.Sequence == sequence))
            throw new KnownException($"工序序号 {sequence} 已存在");

        var operation = new RoutingOperation(sequence, operationName, workCenterId, standardTime);
        Operations.Add(operation);
    }

    public void RemoveOperation(int sequence)
    {
        var operation = Operations.FirstOrDefault(o => o.Sequence == sequence);
        if (operation != null)
        {
            Operations.Remove(operation);
        }
    }
}

/// <summary>
/// 工艺路线工序（子实体）
/// </summary>
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
    public WorkCenterId WorkCenterId { get; private set; }= default!;
    public decimal StandardTime { get; private set; } // 标准工时（小时）
}

public partial record RoutingOperationId : IGuidStronglyTypedId;

