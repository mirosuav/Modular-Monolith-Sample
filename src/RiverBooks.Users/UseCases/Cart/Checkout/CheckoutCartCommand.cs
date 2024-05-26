
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.Cart.Checkout;
public record CheckoutCartCommand(string EmailAddress,
                                  Guid shippingAddressId,
                                  Guid billingAddressId)
    : IRequest<ResultOr<Guid>>;
