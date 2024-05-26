using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, ResultOr>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IMediator _mediator;

    public AddItemToCartHandler(
        IApplicationUserRepository userRepository,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<ResultOr> Handle(AddItemToCartCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.Unauthorized;
        }

        var query = new BookDetailsQuery(request.BookId);

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return result.Errors;

        var bookDetails = result.Value!;

        var description = $"{bookDetails.Title} by {bookDetails.Author}";
        var newCartItem = new CartItem(request.BookId,
          description,
          request.Quantity,
          bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();

        return ResultOr.Success();
    }
}
