using MediatR;

namespace RiverBooks.OrderProcessing.Contracts;

// Todo as this is only used by Reporting module, should be moved there and renamed to BookSale event or command
public class OrderCreatedIntegrationEvent(OrderDto orderDto) : INotification
{
    public OrderDto Order { get; private set; } = orderDto;
}
