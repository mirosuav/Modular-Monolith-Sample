using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace RiverBooks.EmailSending.Api;

internal static class EmailSendingEndpoints
{
    internal static RouteGroupBuilder MapEmailSendingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", ListEmailsAsync).AllowAnonymous();

        return group;
    }

    internal static Task<Ok> ListEmailsAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement paging
        //var filter = Builders<EmailOutboxEntity>.Filter.Empty;
        //var emailEntities = await _emailCollection.Find(filter)
        //  .ToListAsync();

        //var response = new ListEmailsResponse()
        //{
        //    Count = emailEntities.Count,
        //    Emails = emailEntities // TODO: Use a separate DTO
        //};

        //Response = response;
        return Task.FromResult(TypedResults.Ok());
    }
}
