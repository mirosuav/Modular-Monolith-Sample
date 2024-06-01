using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.UseCases.Cart.AddItem;
using RiverBooks.Users.UseCases.Cart.Checkout;
using RiverBooks.Users.UseCases.Cart.ListItems;

namespace RiverBooks.Users.Api;

internal static class CartEndpoints
{
    internal static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", AddItemToCartAsync)
            .Produces<Ok>()
            .Produces<BadRequest>();

        group.MapPost("/checkout", CheckoutCartAsync)
            .Produces<Ok<CheckoutResponse>>()
            .Produces<BadRequest>();

        group.MapGet("", ListCartItemsAsync)
            .Produces<Ok<CartResponse>>()
            .Produces<BadRequest>();

        return group;
    }

    internal static async Task<IResult> AddItemToCartAsync(
        AddCartItemRequest request,
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var command = new AddItemToCartCommand(request.BookId, request.Quantity, emailAddress!);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }

    internal static async Task<IResult> CheckoutCartAsync(
        CheckoutRequest request,
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var command = new CheckoutCartCommand(emailAddress!,
                                              request.ShippingAddressId,
                                              request.BillingAddressId);

        var result = await sender.Send(command, cancellationToken);

        return result.MatchHttpOk(v => new CheckoutResponse(v));
    }

    internal static async Task<IResult> ListCartItemsAsync(
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var query = new ListCartItemsQuery(emailAddress!);

        var result = await sender.Send(query, cancellationToken);

        return result.MatchHttpOk(v => new CartResponse(v));
    }
}
