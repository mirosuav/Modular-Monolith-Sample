﻿using MediatR;
using RiverBooks.Books.Contracts;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.UseCases.Cart.AddItem;

public class AddItemToCartHandler(
    IUserRepository userRepository,
    IMediator mediator) : IRequestHandler<AddItemToCartCommand, ResultOf>
{
    public async Task<ResultOf> Handle(AddItemToCartCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetUserWithCartAsync(request.UserId);

        if (user is null) return Error.NotAuthorized;

        var query = new BookDetailsQuery(request.BookId);

        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
            return new ResultOf(result.Errors);

        var bookDetails = result.Value!;

        var description = $"{bookDetails.Title} by {bookDetails.Author}";
        var newCartItem = new CartItem(
            request.BookId,
            request.UserId,
            description,
            request.Quantity,
            bookDetails.Price);

        user.AddItemToCart(newCartItem);

        await userRepository.SaveChangesAsync(ct);

        return true;
    }
}