using Ardalis.Result;
using FastEndpoints;
using FastEndpoints.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RiverBooks.SharedKernel;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.UseCases.User.AddAddress;
using RiverBooks.Users.UseCases.User.Create;
using RiverBooks.Users.UseCases.User.ListAddresses;

namespace RiverBooks.Users.Api;

internal static class UserEndpoints
{
    internal static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", CreateUserAsync)
            .AllowAnonymous();

        group.MapPost("/login", LoginUserAsync)
            .AllowAnonymous();

        group.MapGet("/addresses", ListUserAdressesAsync);

        group.MapPost("/addresses", AddUserAdressesAsync);

        return group;
    }

    internal static async Task<Results<Ok, BadRequest, ProblemHttpResult>> CreateUserAsync(
        CreateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(request.Email, request.Password);

        var result = await sender.Send(command);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(); //TODO return ProblemDetails
        }

        return TypedResults.Ok();
    }

    internal static async Task<Results<Ok<string>, UnauthorizedHttpResult>> LoginUserAsync(
        UserLoginRequest request,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
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

        var jwtSecret = configuration["Auth:JwtSecret"]!; //TODO move it to separate handler
        var token = JWTBearer.CreateToken(jwtSecret,
          p => p["EmailAddress"] = user.Email!);
        return TypedResults.Ok(token);
    }

    internal static async Task<Results<Ok<AddressListResponse>, UnauthorizedHttpResult>> ListUserAdressesAsync(
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var query = new ListAddressesQuery(emailAddress!);

        var result = await sender.Send(query, cancellationToken);

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        else
        {
            var response = new AddressListResponse();

            response.Addresses = result.Value;

            return TypedResults.Ok(response);
        }
    }

    internal static async Task<Results<Ok, UnauthorizedHttpResult>> AddUserAdressesAsync(
        AddAddressRequest request,
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var command = new AddAddressToUserCommand(emailAddress!,
          request.Street1,
          request.Street2,
          request.City,
          request.State,
          request.PostalCode,
          request.Country);

        var result = await sender.Send(command);

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        else
        {
            return TypedResults.Ok();
        }
    }
}
