using RiverBooks.SharedKernel.DomainEvents;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderCreatedEvent : DomainEventBase
{
    public OrderCreatedEvent(Order order)
    {
        Order = order;
    }

    public Order Order { get; }
}
