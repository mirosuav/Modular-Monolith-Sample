using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal class OrderCreatedDomainEvent(Order order, DateTime occuredUtc) : DomainEventBase(occuredUtc)
{
    public Order Order { get; } = order;
}
