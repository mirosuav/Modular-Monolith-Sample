using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.EmailSending.Domain;

namespace RiverBooks.EmailSending.Api;

internal static class EmailSendingEndpoints
{
    internal static RouteGroupBuilder MapEmailSendingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", ListEmailsAsync)
            .Produces<Ok>();

        return group;
    }

    internal static Task<IResult> ListEmailsAsync(
        CancellationToken cancellationToken = default)
    {        
        return Task.FromResult(Results.Ok("Not implemented"));
    }
}
