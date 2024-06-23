
using MediatR;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

// TODO Use UserId instead of EmailAddress
public record CheckoutCartCommand(string EmailAddress,
                                  Guid ShippingAddressId,
                                  Guid BillingAddressId)
    : IRequest<Resultable<Guid>>;
