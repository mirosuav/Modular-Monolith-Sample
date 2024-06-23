using Microsoft.EntityFrameworkCore;
using RiverBooks.EmailSending.Domain;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.EmailSending.Data;

internal class EmailOutboxRepository(EmailSendingDbContext dbContext, TimeProvider timeProvider) :
    IGetEmailsFromOutboxService,
    IQueueEmailsInOutboxService,
    IMarkEmailProcessed
{
    private readonly EmailSendingDbContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Resultable<EmailOutboxEntity>> GetUnprocessedEmailEntity(CancellationToken cancellationToken)
    {
        var result = await _dbContext.EmailOutboxItems
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
        return _dbContext.EmailOutboxItems
            .Where(x => x.Id == emailId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(e => e.DateTimeUtcProcessed, 
                _timeProvider.GetUtcNow().DateTime), 
                cancellationToken);
    }

    public async Task QueueEmailForSending(EmailOutboxEntity entity, CancellationToken cancellationToken)
    {
        PassOrThrow.IfNull(entity);
        _dbContext.EmailOutboxItems.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
