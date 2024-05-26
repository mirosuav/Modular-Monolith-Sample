
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace OrderProcessing.Contracts;

public record CreateOrderCommand(Guid UserId,
                                 Guid ShippingAddressId,
                                 Guid BillingAddressId,
                                 List<OrderItemDetails> OrderItems) :
    IRequest<ResultOr<OrderDetailsResponse>>;
