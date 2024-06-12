using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.UseCases.User.AddAddress;
using RiverBooks.Users.UseCases.User.Create;
using RiverBooks.Users.UseCases.User.Delete;
using RiverBooks.Users.UseCases.User.ListAddresses;

namespace RiverBooks.Users.Api;

internal static class UserEndpoints
{
    internal static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", CreateUserAsync)
            .Produces<Ok>()
            .Produces<BadRequest>()
            .AllowAnonymous();

        group.MapPost("/login", LoginUserAsync)
            .Produces<Ok<string>>()
            .Produces<UnauthorizedHttpResult>()
            .Produces<BadRequest>()
            .AllowAnonymous();

        group.MapGet("/addresses", ListUserAdressesAsync)
            .Produces<Ok<AddressListResponse>>()
            .Produces<BadRequest>();

        group.MapPost("/addresses", AddUserAdressesAsync)
            .Produces<Ok>()
            .Produces<BadRequest>();

        group.MapDelete("", DeleteUserAsync)
            .Produces<Ok>()
            .Produces<NotFound>();

        return group;
    }

    internal static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(request.Email, request.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }

    internal static async Task<IResult> LoginUserAsync(
        UserLoginRequest request,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] IJwtTokenHandler jwtTokenHandler,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email!);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var loginSuccessful = await userManager.CheckPasswordAsync(user, request.Password);

        if (!loginSuccessful)
        {
            return TypedResults.Unauthorized();
        }

        var token = jwtTokenHandler.CreateToken(user.Id.ToString(), user.Email!);

        return TypedResults.Ok(token);
    }

    internal static async Task<IResult> ListUserAdressesAsync(
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetEmailAddress();

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var query = new ListAddressesQuery(emailAddress!);

        var result = await sender.Send(query, cancellationToken);

        return result.MatchHttpOk(r => new AddressListResponse(r));
    }

    internal static async Task<IResult> AddUserAdressesAsync(
        AddAddressRequest request,
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetEmailAddress();

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var command = new AddAddressToUserCommand(emailAddress!,
          request.Street1,
          request.Street2,
          request.City,
          request.State,
          request.PostalCode,
          request.Country);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }


    internal static async Task<IResult> DeleteUserAsync(
        [FromServices] IUserClaimsProvider userClaimsProvider,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var command = new DeleteUserCommand(userId.Value);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpOk();
    }

}
