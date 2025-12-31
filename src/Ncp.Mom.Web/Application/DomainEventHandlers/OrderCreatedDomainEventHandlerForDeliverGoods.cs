using Ncp.Mom.Domain.DomainEvents;
using Ncp.Mom.Web.Application.Commands.Delivers;

namespace Ncp.Mom.Web.Application.DomainEventHandlers;

public class OrderCreatedDomainEventHandlerForDeliverGoods(IMediator mediator) : IDomainEventHandler<OrderCreatedDomainEvent>
{
    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return mediator.Send(new DeliverGoodsCommand(notification.Order.Id), cancellationToken);
    }
}