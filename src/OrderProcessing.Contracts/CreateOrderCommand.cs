
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace OrderProcessing.Contracts;

public record CreateOrderCommand(string UserId,
                                 Guid ShippingAddressId,
                                 Guid BillingAddressId,
                                 List<OrderItemDetails> OrderItems) :
    IRequest<Resultable<OrderDetailsResponse>>;
