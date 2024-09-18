using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Application.UseCases.Cart.Checkout;

public record CheckoutCartCommand(
    Guid UserId,
    Guid ShippingAddressId,
    Guid BillingAddressId)
    : IRequest<ResultOf<Guid>>;