using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.OrderProcessing.Domain;

internal class Order : HaveEvents
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } = default!;
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;

    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

    private void AddOrderItem(OrderItem item) => _orderItems.Add(item);
    
    public static Order CreateNew(
        Guid userId,
        Address shippingAddress,
        Address billingAddress,
        IEnumerable<OrderItem> orderItems,
        TimeProvider timeProvider)
    {
        var order = new Order
        {
            Id = SequentialGuid.NewGuid(),
            UserId = userId,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress
        };

        foreach (var item in orderItems)
        {
            order.AddOrderItem(item);
        }

        var createdEvent = new OrderCreatedDomainEvent(order, timeProvider.GetUtcDateTime());

        order.RegisterDomainEvent(createdEvent);

        return order;
    }
}



