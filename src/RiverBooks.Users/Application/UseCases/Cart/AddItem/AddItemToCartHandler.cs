using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler(
    IApplicationUserRepository userRepository,
    IMediator mediator) : IRequestHandler<AddItemToCartCommand, Resultable>
{
    public async Task<Resultable> Handle(AddItemToCartCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.NotAuthorized;
        }

        var query = new BookDetailsQuery(request.BookId);

        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
            return result.Errors;

        var bookDetails = result.Value!;

        var description = $"{bookDetails.Title} by {bookDetails.Author}";
        var newCartItem = new CartItem(request.BookId,
          description,
          request.Quantity,
          bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await userRepository.SaveChangesAsync(ct);

        return Resultable.Success();
    }
}
