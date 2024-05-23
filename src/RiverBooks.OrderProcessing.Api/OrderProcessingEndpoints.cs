using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.OrderProcessing.Endpoints;
using RiverBooks.OrderProcessing.ListOrdersForUser;

namespace RiverBooks.OrderProcessing.Api;

internal static class OrderProcessingEndpoints
{
    internal static RouteGroupBuilder MapOrderProcessingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", ListOrdersForUserAsync);

        return group;
    }


    internal static async Task<Results<Ok<ListOrdersForUserResponse>, UnauthorizedHttpResult>> ListOrdersForUserAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var emailAddress = ""; // TODO User.FindFirstValue("EmailAddress");

        var query = new ListOrdersForUserQuery(emailAddress!);

        var result = await sender.Send(query, cancellationToken);

        if (result.Status == ResultStatus.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }
        else
        {
            var response = new ListOrdersForUserResponse();
            response.Orders = result.Value.Select(o => new OrderSummary
            {
                DateCreated = o.DateCreated,
                DateShipped = o.DateShipped,
                Total = o.Total,
                UserId = o.UserId,
                OrderId = o.OrderId
            }).ToList();

            return TypedResults.Ok(response);
        }
    }


}
