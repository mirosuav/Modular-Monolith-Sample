using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Integrations;
internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,
  ResultOr<OrderDetailsResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IOrderAddressCache _addressCache;

    public CreateOrderCommandHandler(IOrderRepository orderRepository,
      ILogger<CreateOrderCommandHandler> logger,
      IOrderAddressCache addressCache)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _addressCache = addressCache;
    }

    public async Task<ResultOr<OrderDetailsResponse>> Handle(CreateOrderCommand request,
      CancellationToken cancellationToken)
    {
        var items = request.OrderItems.Select(oi => new OrderItem(
          oi.BookId, oi.Quantity, oi.UnitPrice, oi.Description));

        var shippingAddress = await _addressCache.GetByIdAsync(request.ShippingAddressId);
        if (!shippingAddress.IsSuccess)
            return shippingAddress.Errors;

        var billingAddress = await _addressCache.GetByIdAsync(request.BillingAddressId);
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
