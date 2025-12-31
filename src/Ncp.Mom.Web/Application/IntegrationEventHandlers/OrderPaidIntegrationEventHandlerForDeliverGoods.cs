using Ncp.Mom.Web.Application.Commands.Delivers;
using Ncp.Mom.Web.Application.IntegrationEvents;

namespace Ncp.Mom.Web.Application.IntegrationEventHandlers;

public class OrderPaidIntegrationEventHandlerForDeliverGoods(IMediator mediator) : IIntegrationEventHandler<OrderPaidIntegrationEvent>
{
    public async Task HandleAsync(OrderPaidIntegrationEvent eventData, CancellationToken cancellationToken = default)
    {
        var cmd = new DeliverGoodsCommand(eventData.OrderId);
        _ = await mediator.Send(cmd, cancellationToken);
    }
}