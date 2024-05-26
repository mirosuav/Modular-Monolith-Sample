using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.ListItems;

public class ListCartItemsQueryHandler : IRequestHandler<ListCartItemsQuery, ResultOr<List<CartItemDto>>>
{
    private readonly IApplicationUserRepository _userRepository;

    public ListCartItemsQueryHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResultOr<List<CartItemDto>>> Handle(ListCartItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.Unauthorized;
        }

        return user.CartItems
          .Select(item => new CartItemDto(item.Id, item.BookId,
          item.Description, item.Quantity, item.UnitPrice))
          .ToList();
    }
}
