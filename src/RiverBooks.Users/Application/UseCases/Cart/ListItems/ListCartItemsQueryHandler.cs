using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

public class ListCartItemsQueryHandler(IApplicationUserRepository userRepository) : IRequestHandler<ListCartItemsQuery, Resultable<List<CartItemDto>>>
{
    public async Task<Resultable<List<CartItemDto>>> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.NotAuthorized;
        }

        return user.CartItems
          .Select(item => new CartItemDto(item.Id, item.BookId,
          item.Description, item.Quantity, item.UnitPrice))
          .ToList();
    }
}
