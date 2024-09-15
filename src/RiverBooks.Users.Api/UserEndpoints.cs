using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Application.UseCases.User.AddAddress;
using RiverBooks.Users.Application.UseCases.User.Create;
using RiverBooks.Users.Application.UseCases.User.Delete;
using RiverBooks.Users.Application.UseCases.User.ListAddresses;
using RiverBooks.Users.Application.UseCases.User.Login;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Api;

internal static class UserEndpoints
{
    internal static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", CreateUserAsync)
            .Produces<Created>()
            .Produces<BadRequest>()
            .AllowAnonymous();

        group.MapPost("/login", LoginUserAsync)
            .Produces<Ok<string>>()
            .Produces<UnauthorizedHttpResult>()
            .Produces<BadRequest>()
            .AllowAnonymous();

        group.MapGet("/addresses", ListUserAddressesAsync)
            .Produces<Ok<AddressListResponse>>()
            .Produces<BadRequest>();

        group.MapPost("/addresses", AddUserAddressesAsync)
            .Produces<Created<Guid>>()
            .Produces<BadRequest>();

        group.MapDelete("", DeleteUserAsync)
            .Produces<Ok>()
            .Produces<NotFound>();

        return group;
    }

    internal static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        return (await sender.Send(
                new CreateUserCommand(request.Email, request.Password),
                cancellationToken))
            .ToHttpCreated();
    }

    internal static async Task<IResult> LoginUserAsync(
        UserLoginRequest request,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await sender.Send(command, cancellationToken);
        return result.ToHttpOk();
    }

    internal static async Task<IResult> ListUserAddressesAsync(
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var query = new ListAddressesQuery(userId.Value);

        var result = await sender.Send(query, cancellationToken);

        return result.ToHttpOk(r => new AddressListResponse(r));
    }

    internal static async Task<IResult> AddUserAddressesAsync(
        AddAddressRequest request,
        ISender sender,
        IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null)
            return TypedResults.Unauthorized();

        var command = new AddAddressToUserCommand(
            userId.Value,
            request.Street1,
            request.Street2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpCreated();
    }

    internal static async Task<IResult> DeleteUserAsync(
        IUserClaimsProvider userClaimsProvider,
        ISender sender,
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
