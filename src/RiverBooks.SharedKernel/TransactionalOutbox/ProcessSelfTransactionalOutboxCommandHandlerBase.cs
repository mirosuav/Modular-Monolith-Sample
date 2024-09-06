using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.SharedKernel.TransactionalOutbox;

public abstract class ProcessSelfTransactionalOutboxCommandHandlerBase
    <TOutboxContext>(
        TOutboxContext dbContext,
        IPublisher publisher,
        ILogger logger,
        TimeProvider timeProvider) :
    INotificationHandler<ProcessSelfTransactionalOutboxCommand>
    where TOutboxContext : TransactionalOutboxDbContext
{
    protected abstract SemaphoreSlim AccessLocker { get; }

    public async Task Handle(ProcessSelfTransactionalOutboxCommand command, CancellationToken ct)
    {
        await AccessLocker.WaitAsync(ct);

        try
        {
            // retrieve outbox events from db
            var outboxItems = await dbContext.FetchNextTransactionalOutboxEvents().ToListAsync(ct);

            if (outboxItems is null or [])
                return;

            // parse, deserialize and process events
            foreach (var outboxEvent in outboxItems)
            {
                await ProcessOutboxEvent(outboxEvent, command.Id, ct);
            }

            // save changes to outbox events
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Processing transactional events [{ProcessDomainEventsSession}] failed `{ErrorMessage}`.",
                command.Id, ex.Message);
            throw;
        }
        finally
        {
            AccessLocker.Release();
        }
    }

    private async Task ProcessOutboxEvent(TransactionalOutboxEvent transactionalOutbox, Guid processingId, CancellationToken cancellationToken)
    {
        try
        {
            transactionalOutbox.Attempts++;
            transactionalOutbox.ProcessedUtc = timeProvider.GetUtcDateTime();
            logger.LogTrace(
                "Processing events [{ProcessDomainEventsSession}] TransactionalOutboxEvent {TransactionalOutboxEvent.Id}, attempt {Attempt}",
                processingId, transactionalOutbox.Id, transactionalOutbox.Attempts);

            var eventType = Type.GetType(transactionalOutbox.EventType);
            if (eventType is null)
            {
                logger.LogError(
                    "Processing events [{ProcessDomainEventsSession}]: Could not create DomainEvent of type `{TransactionalOutboxEvent}`.",
                    processingId, transactionalOutbox.EventType);
                return;
            }

            var domainEvent = JsonSerializer.Deserialize(transactionalOutbox.Payload, eventType);
            if (domainEvent is null)
            {
                logger.LogError(
                    "Processing events [{ProcessDomainEventsSession}]: Could not create DomainEvent of type `{DomainEvent}`.",
                    processingId, transactionalOutbox.EventType);
                return;
            }

            await publisher.Publish(domainEvent, cancellationToken);
            transactionalOutbox.Success = true;

            logger.LogTrace(
                "Processing events [{ProcessDomainEventsSession}] TransactionalOutboxEvent {TransactionalOutboxEvent.Id}, attempt {Attempt} succeeded.",
                processingId, transactionalOutbox.Id, transactionalOutbox.Attempts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Processing events [{ProcessDomainEventsSession}]: Could not publish DomainEvent of type `{DomainEvent}` and Id `{DomainEvent.Id}`.",
                processingId, transactionalOutbox.EventType, transactionalOutbox.Id);
            return;
        }
    }
}

