using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Integrations;
internal class CreateOrderCommandHandler(IOrderRepository orderRepository,
  ILogger<CreateOrderCommandHandler> logger,
  IOrderAddressCache addressCache) : IRequestHandler<CreateOrderCommand,
  Resultable<OrderDetailsResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
    private readonly IOrderAddressCache _addressCache = addressCache;

    public async Task<Resultable<OrderDetailsResponse>> Handle(CreateOrderCommand request,
      CancellationToken cancellationToken)
    {
        var items = request.OrderItems.Select(oi => new OrderItem(
          oi.BookId, oi.Quantity, oi.UnitPrice, oi.Description));

        var shippingAddress = await _addressCache.GetByIdAsync(request.ShippingAddressId, cancellationToken);
        if (!shippingAddress.IsSuccess)
            return shippingAddress.Errors;

        var billingAddress = await _addressCache.GetByIdAsync(request.BillingAddressId, cancellationToken);
        if (!billingAddress.IsSuccess)
            return billingAddress.Errors;

        var newOrder = Order.Factory.Create(request.UserId,
          shippingAddress.Value.Address,
          billingAddress.Value.Address,
          items);

        await _orderRepository.AddAsync(newOrder);
        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation("New Order Created! {orderId}", newOrder.Id);

        return new OrderDetailsResponse(newOrder.Id);
    }
}
