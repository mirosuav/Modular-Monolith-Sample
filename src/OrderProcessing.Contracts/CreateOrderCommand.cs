using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Contracts;

public record CreateOrderCommand(Guid UserId,
                                 Guid ShippingAddressId,
                                 Guid BillingAddressId,
                                 List<OrderItemDto> OrderItems) :
    IRequest<Resultable<OrderDetailsResponse>>;
