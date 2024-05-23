using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.UseCases.Cart.AddItem;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result<CartItem>>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public AddItemToCartHandler(IApplicationUserRepository userRepository,
      IMediator mediator,
      IServiceProvider serviceProvider)
    {
        _userRepository = userRepository;
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<CartItem>> Handle(AddItemToCartCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Error.UserNotAauthorized;
        }

        // Use reflection to call a method on a dynamically loaded assembly
        //var dynamicResult = await GetBookByIdAsync(request.BookId);

        //string description = $"{dynamicResult.Title} by {dynamicResult.Author}";

        //var newCartItem = new CartItem(request.BookId,
        //  description,
        //  request.Quantity,
        //  dynamicResult.Price);

        var query = new BookDetailsQuery(request.BookId);

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return result.Errors!;

        var bookDetails = result.Value!;

        var description = $"{bookDetails.Title} by {bookDetails.Author}";
        var newCartItem = new CartItem(request.BookId,
          description,
          request.Quantity,
          bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();

        return newCartItem;
    }
}
