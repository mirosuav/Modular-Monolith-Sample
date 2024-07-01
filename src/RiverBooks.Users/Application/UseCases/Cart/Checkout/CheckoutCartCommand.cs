
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

// TODO Use UserId instead of Email
public record CheckoutCartCommand(string EmailAddress,
                                  Guid ShippingAddressId,
                                  Guid BillingAddressId)
    : IRequest<Resultable<Guid>>;
