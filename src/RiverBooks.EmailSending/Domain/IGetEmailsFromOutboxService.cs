using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Domain;

internal interface IGetEmailsFromOutboxService
{
    Task<ResultOf<EmailOutboxEntity>> GetNextUnprocessedEmailEntity(CancellationToken cancellationToken);
    Task<ResultOf<List<EmailOutboxEntity>>> GetAllUnprocessedEmailsEntities(CancellationToken cancellationToken);
    Task<ResultOf<List<EmailOutboxEntity>>> GetAllProcessedEmailsEntities(CancellationToken cancellationToken);
}