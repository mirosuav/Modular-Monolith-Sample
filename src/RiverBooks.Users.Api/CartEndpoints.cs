using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.SharedKernel;
using RiverBooks.Users.CartEndpoints;
using RiverBooks.Users.UseCases.Cart.AddItem;
using RiverBooks.Users.UseCases.Cart.Checkout;
using RiverBooks.Users.UseCases.Cart.ListItems;

namespace RiverBooks.Users.Api;

internal static class CartEndpoints
{
    internal static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", AddItemToCartAsync);

        group.MapPost("/checkout", CheckoutCartAsync);

        group.MapGet("", ListCartItemsAsync);

        return group;
    }

    internal static async Task<Results<Ok, UnauthorizedHttpResult, BadRequest, ProblemHttpResult>> AddItemToCartAsync(
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

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        if (result.Status == ResultStatus.Invalid)
        {
            return TypedResults.BadRequest(); //TODO return ProblemDetails
        }
        else
        {
            return TypedResults.Ok();
        }
    }

    internal static async Task<Results<Ok<CheckoutResponse>, UnauthorizedHttpResult, ProblemHttpResult>> CheckoutCartAsync(
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

        var result = await sender.Send(command);

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        else
        {
            return TypedResults.Ok(new CheckoutResponse(result.Value));
        }
    }

    internal static async Task<Results<Ok<CartResponse>, UnauthorizedHttpResult, ProblemHttpResult>> ListCartItemsAsync(
        ISender sender, 
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var query = new ListCartItemsQuery(emailAddress!);

        var result = await sender.Send(query);

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        else
        {
            var response = new CartResponse()
            {
                CartItems = result.Value
            };
            return TypedResults.Ok(response);
        }
    }
}
