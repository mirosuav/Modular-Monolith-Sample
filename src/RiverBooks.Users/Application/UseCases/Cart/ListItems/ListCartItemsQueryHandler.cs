using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Application.UseCases.Cart.ListItems;

public class ListCartItemsQueryHandler(IApplicationUserRepository userRepository) : IRequestHandler<ListCartItemsQuery, Resultable<List<CartItemDto>>>
{
    public async Task<Resultable<List<CartItemDto>>> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithCartAsync(request.UserId);

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
