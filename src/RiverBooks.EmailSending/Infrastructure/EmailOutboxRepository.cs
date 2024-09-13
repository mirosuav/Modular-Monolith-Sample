using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Domain;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Infrastructure;

internal class EmailOutboxRepository(EmailSendingDbContext dbContext, TimeProvider timeProvider, ILogger<EmailOutboxRepository> logger) :
    IGetEmailsFromOutboxService,
    IQueueEmailsInOutboxService,
    IMarkEmailProcessed
{
    public async Task<ResultOf<List<EmailOutboxEntity>>> GetAllUnprocessedEmailsEntities(CancellationToken cancellationToken)
    {
        return await dbContext.EmailOutboxItems
           .AsNoTracking()
           .Where(x => x.Status == EmailProcessingStatus.Pending)
           .OrderBy(x => x.Id)
           .ToListAsync(cancellationToken);
    }

    public async Task<ResultOf<EmailOutboxEntity>> GetNextUnprocessedEmailEntity(CancellationToken cancellationToken)
    {
        var result = await dbContext.EmailOutboxItems
            .AsNoTracking()
            .Where(x => x.Status == EmailProcessingStatus.Pending)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            return Error.NotFound();

        return result;
    }

    public async Task<ResultOf<List<EmailOutboxEntity>>> GetAllProcessedEmailsEntities(CancellationToken cancellationToken)
    {
        return await dbContext.EmailOutboxItems
            .AsNoTracking()
            .Where(x => x.Status != EmailProcessingStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateEmailStatus(Guid emailId, EmailProcessingStatus status, CancellationToken cancellationToken)
    {
        return dbContext.EmailOutboxItems
            .Where(x => x.Id == emailId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(e => e.ProcessedAtUtc, timeProvider.GetUtcDateTime())
                    .SetProperty(e => e.Status, status),
                cancellationToken);
    }

    public async Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken cancellationToken)
    {
        dbContext.EmailOutboxItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Email {entity.Id} queued in outbox...");
    }
}
