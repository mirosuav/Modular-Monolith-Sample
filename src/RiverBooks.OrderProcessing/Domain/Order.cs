using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Events;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.OrderProcessing.Domain;

internal class Order : HaveEvents
{
    private readonly List<OrderItem> _orderItems = [];
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.Now;

    private void AddOrderItem(OrderItem item)
    {
        _orderItems.Add(item);
    }

    public static Order Create(
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
            BillingAddress = billingAddress,
            DateCreated = timeProvider.GetUtcDateTime()
        };

        foreach (var item in orderItems) order.AddOrderItem(item);

        order.RegisterEvent(new PrepareReportForOrderEvent(order.Id, order.DateCreated));
        order.RegisterEvent(new SendOrderConfirmationEmailEvent(order.Id, order.DateCreated));

        return order;
    }

    public OrderDto ToOrderDto()
    {
        return new OrderDto
        {
            DateCreated = DateCreated,
            OrderId = Id,
            UserId = UserId,
            OrderItems = OrderItems
                .Select(oi => new OrderItemDto(
                    oi.BookId,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.Description))
                .ToList()
        };
    }
}