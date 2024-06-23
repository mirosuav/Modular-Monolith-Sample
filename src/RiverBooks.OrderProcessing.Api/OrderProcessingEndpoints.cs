using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RiverBooks.OrderProcessing.Endpoints;
using RiverBooks.OrderProcessing.ListOrdersForUser;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Api;

internal static class OrderProcessingEndpoints
{
    internal static RouteGroupBuilder MapOrderProcessingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", ListOrdersForUserAsync)
            .Produces<Ok<ListOrdersForUserResponse>>()
            .Produces<UnauthorizedHttpResult>();

        return group;
    }

    internal static async Task<IResult> ListOrdersForUserAsync(
        [FromServices] ISender sender,
        [FromServices] IUserClaimsProvider userClaimsProvider,
        CancellationToken cancellationToken)
    {
        var userId = userClaimsProvider.GetId();

        if (userId is null or [])
            return TypedResults.Unauthorized();

        var query = new ListOrdersForUserQuery(userId);

        var result = await sender.Send(query, cancellationToken);

        return result.MatchHttpOk(v => new ListOrdersForUserResponse(v));
    }
}
