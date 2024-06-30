using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Domain;

internal interface IGetEmailsFromOutboxService
{
    Task<Resultable<EmailOutboxEntity>> GetNextUnprocessedEmailEntity(CancellationToken cancellationToken);
    Task<Resultable<List<EmailOutboxEntity>>> GetAllUnprocessedEmailsEntities(CancellationToken cancellationToken);
}
