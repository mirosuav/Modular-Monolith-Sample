using FastEndpoints.Security;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
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

        return group;
    }

    internal static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(request.Email, request.Password);

        var result = await sender.Send(command);

        return result.ToHttpOk();
    }

    internal static async Task<IResult> LoginUserAsync(
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

    internal static async Task<IResult> ListUserAdressesAsync(
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = userClaimsProvider.GetClaim("EmailAddress");

        if (emailAddress is null)
            return TypedResults.Unauthorized();

        var query = new ListAddressesQuery(emailAddress!);

        var result = await sender.Send(query, cancellationToken);

        return result.MatchHttpOk(r => new AddressListResponse(r));
    }

    internal static async Task<IResult> AddUserAdressesAsync(
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

        return result.ToHttpOk();
    }
}
