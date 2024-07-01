using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.Cart.Checkout;

public class CheckoutCartHandler(IApplicationUserRepository userRepository,
  IMediator mediator) : IRequestHandler<CheckoutCartCommand, Resultable<Guid>>
{
    public async Task<Resultable<Guid>> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithCartAsync(request.UserId);

        if (user is null)
        {
            return Error.NotAuthorized;
        }

        var items = user.CartItems.Select(item =>
          new OrderItemDetails(item.BookId,
                               item.Quantity,
                               item.UnitPrice,
                               item.Description))
          .ToList();

        var createOrderCommand = new CreateOrderCommand(user.Id,
          request.ShippingAddressId,
          request.BillingAddressId,
          items);

        // TODO: Consider replacing with a message-based approach for perf reasons
        var result = await mediator.Send(createOrderCommand, cancellationToken); // synchronous

        if (!result.IsSuccess)
        {
            return result.Errors;
        }

        user.ClearCart();
        await userRepository.SaveChangesAsync(cancellationToken);

        return Resultable.Success(result.Value.OrderId);
    }
}
