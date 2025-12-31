using Ncp.Mom.Domain.AggregatesModel.OrderAggregate;

namespace Ncp.Mom.Web.Application.IntegrationEvents;

public record OrderPaidIntegrationEvent(OrderId OrderId);
