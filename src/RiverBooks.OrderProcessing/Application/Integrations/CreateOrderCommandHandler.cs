using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Application.Integrations;
internal class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ILogger<CreateOrderCommandHandler> logger,
    IOrderAddressCache addressCache,
    TimeProvider timeProvider)
    : IRequestHandler<CreateOrderCommand, Resultable<OrderDetailsResponse>>
{
    public async Task<Resultable<OrderDetailsResponse>> Handle(CreateOrderCommand request,
      CancellationToken cancellationToken)
    {
        var items = request.OrderItems.Select(oi => new OrderItem(
          oi.BookId, oi.Quantity, oi.UnitPrice, oi.Description));

        var shippingAddress = await addressCache.GetByIdAsync(request.ShippingAddressId, cancellationToken);
        if (!shippingAddress.IsSuccess)
            return shippingAddress.Errors;

        var billingAddress = await addressCache.GetByIdAsync(request.BillingAddressId, cancellationToken);
        if (!billingAddress.IsSuccess)
            return billingAddress.Errors;

        var newOrder = Order.Create(request.UserId,
          shippingAddress.Value.Address,
          billingAddress.Value.Address,
          items,
          timeProvider);

        orderRepository.Add(newOrder);
        await orderRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("New Order Created! {orderId}", newOrder.Id);

        return new OrderDetailsResponse(newOrder.Id);
    }
}
