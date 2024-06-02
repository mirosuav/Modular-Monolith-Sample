using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Domain;

internal interface IGetEmailsFromOutboxService
{
    Task<Resultable<EmailOutboxEntity>> GetUnprocessedEmailEntity(CancellationToken cancellationToken);
}
