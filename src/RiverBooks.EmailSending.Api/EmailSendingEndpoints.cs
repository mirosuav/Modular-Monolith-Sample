using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RiverBooks.EmailSending.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Api;

internal static class EmailSendingEndpoints
{
    internal static RouteGroupBuilder MapEmailSendingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", ListEmailsAsync)
            .Produces<Ok<List<EmailOutboxEntity>>>();

        return group;
    }

    internal static async Task<IResult> ListEmailsAsync(
        IGetEmailsFromOutboxService getEmailsFromOutboxService,
        CancellationToken cancellationToken = default)
    {
        return (await getEmailsFromOutboxService
            .GetAllUnprocessedEmailsEntities(cancellationToken))
            .MatchHttpOk(r => r);
    }
}
