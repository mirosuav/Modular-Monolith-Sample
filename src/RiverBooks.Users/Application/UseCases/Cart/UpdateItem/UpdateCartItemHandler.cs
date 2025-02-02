using MediatR;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;

namespace RiverBooks.Users.Application.UseCases.Cart.UpdateItem;

public class UpdateCartItemHandler(IUserRepository userRepository) 
    : IRequestHandler<UpdateCartItemCommand, ResultOf>
{
    public async Task<ResultOf> Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetUserWithCartAsync(request.UserId);

        if (user is null) return Error.NotAuthorized;

        var cartItem = user.CartItems.FirstOrDefault(i => i.Id == request.ItemId);

        if (cartItem is null)
            return Error.NotFound("Cart item not found.");

        if (request.Quantity == 0)
            user.RemoveItemFromCart(cartItem);
        else
        {
            cartItem.UpdateQuantity(request.Quantity);
        }

        await userRepository.SaveChangesAsync(ct);

        return true;
    }
}