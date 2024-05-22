using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.UseCases.User.Create;

namespace RiverBooks.Users.Api;

internal static class UserEndpoints
{
  internal static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
  {
    group.MapPost("", static async (CreateUserRequest request, ISender sender, CancellationToken cancellationToken) =>
    {
      var command = new CreateUserCommand(request.Email, request.Password);

      var result = await sender.Send(command);

      if (!result.IsSuccess)
      {
        // Todo handle failure
        return Results.BadRequest();
      }
      return Results.Ok();
    });

    return group;
  }
}
