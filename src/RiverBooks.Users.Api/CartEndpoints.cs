using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.UseCases.Cart.AddItem;
using RiverBooks.Users.Application.UseCases.Cart.Checkout;
using RiverBooks.Users.Application.UseCases.Cart.ListItems;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Api;

internal static class CartEndpoints
{
    internal static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", AddItemToCartAsync)
            .Produces<Ok>()
            .Produces<BadRequest>();

        group.MapPost("/checkout", CheckoutCartAsync)
            .Produces<Ok<Guid>>()
            .Produces<BadRequest>();

        group.MapGet("", ListCartItemsAsync)
            .Produces<Ok<CartResponse>>()
            .Produces<BadRequest>();

        return group;
    }

    internal static async Task<IResult> AddItemToCartAsync(
        AddCartItemRequest request,
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var command = new AddItemToCartCommand(request.BookId, request.Quantity, userId.Value);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }

    internal static async Task<IResult> CheckoutCartAsync(
        CheckoutRequest request,
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var command = new CheckoutCartCommand(userId.Value,
                                              request.ShippingAddressId,
                                              request.BillingAddressId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }

    internal static async Task<IResult> ListCartItemsAsync(
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var query = new ListCartItemsQuery(userId.Value);

        var result = await sender.Send(query, cancellationToken);

        return result.ToHttpOk(v => new CartResponse(v));
    }
}
