using RiverBooks.SharedKernel.DomainEvents;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderCreatedDomainEvent(Order order) : DomainEventBase
{
    public Order Order { get; } = order;
}
