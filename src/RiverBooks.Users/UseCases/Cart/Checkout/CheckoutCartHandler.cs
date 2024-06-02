using MediatR;
using OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.Checkout;

public class CheckoutCartHandler(IApplicationUserRepository userRepository,
  IMediator mediator) : IRequestHandler<CheckoutCartCommand, Resultable<Guid>>
{
    private readonly IApplicationUserRepository _userRepository = userRepository;
    private readonly IMediator _mediator = mediator;

    public async Task<Resultable<Guid>> Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

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

        var createOrderCommand = new CreateOrderCommand(Guid.Parse(user.Id),
          request.ShippingAddressId,
          request.BillingAddressId,
          items);

        // TODO: Consider replacing with a message-based approach for perf reasons
        var result = await _mediator.Send(createOrderCommand, cancellationToken); // synchronous

        if (!result.IsSuccess)
        {
            return Resultable.Failure<Guid>(result.Errors);
        }

        user.ClearCart();
        await _userRepository.SaveChangesAsync();

        return Resultable.Success(result.Value.OrderId);
    }
}
