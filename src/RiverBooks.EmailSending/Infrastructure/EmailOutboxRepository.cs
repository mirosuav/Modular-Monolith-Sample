using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Infrastructure;

internal class EmailOutboxRepository(EmailSendingDbContext dbContext, TimeProvider timeProvider) :
    IGetEmailsFromOutboxService,
    IQueueEmailsInOutboxService,
    IMarkEmailProcessed
{
    public async Task<Resultable<List<EmailOutboxEntity>>> GetAllUnprocessedEmailsEntities(CancellationToken cancellationToken)
    {
        return await dbContext.EmailOutboxItems
           .AsNoTracking()
           .Where(x => x.DateTimeUtcProcessed == null)
           .OrderBy(x => x.Id)
           .ToListAsync(cancellationToken);
    }

    public async Task<Resultable<EmailOutboxEntity>> GetNextUnprocessedEmailEntity(CancellationToken cancellationToken)
    {
        var result = await dbContext.EmailOutboxItems
            .AsNoTracking()
            .Where(x => x.DateTimeUtcProcessed == null)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            return Error.NotFound();

        return result;
    }

    public Task MarkEmailSend(Guid emailId, CancellationToken cancellationToken)
    {
        return dbContext.EmailOutboxItems
            .Where(x => x.Id == emailId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(e => e.DateTimeUtcProcessed,
                timeProvider.GetUtcNow().DateTime),
                cancellationToken);
    }

    public async Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken cancellationToken)
    {
        PassOrThrowWhen.Null(entity);
        dbContext.EmailOutboxItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
