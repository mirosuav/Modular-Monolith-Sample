using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Application.Integrations;
internal class PublishCreatedOrderIntegrationEventHandler(IMediator mediator) :
  INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var dto = new OrderDetailsDto()
        {
            DateCreated = notification.Order.DateCreated,
            OrderId = notification.Order.Id,
            UserId = notification.Order.UserId,
            OrderItems = notification.Order.OrderItems
          .Select(oi => new OrderItemDetails(oi.BookId,
                                             oi.Quantity,
                                             oi.UnitPrice,
                                             oi.Description))
          .ToList()
        };
        var integrationEvent = new OrderCreatedIntegrationEvent(dto);

        await mediator.Publish(integrationEvent, cancellationToken);

    }
}
