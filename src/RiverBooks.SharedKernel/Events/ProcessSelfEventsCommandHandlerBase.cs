using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.SharedKernel.Events;

public abstract class ProcessSelfEventsCommandHandlerBase
    <TOutboxContext>(
        TOutboxContext dbContext,
        IMediator mediator,
        ILogger logger,
        TimeProvider timeProvider) :
    INotificationHandler<ProcessSelfEventsCommand>
    where TOutboxContext : TransactionalOutboxDbContext
{
    protected readonly TOutboxContext DbContext = dbContext;
    protected readonly IMediator Mediator = mediator;
    protected readonly ILogger Logger = logger;
    protected readonly TimeProvider TimeProvider = timeProvider;
    protected abstract SemaphoreSlim AccessLocker { get; }

    public async Task Handle(ProcessSelfEventsCommand command, CancellationToken ct)
    {
        await AccessLocker.WaitAsync(ct);

        try
        {
            // retrieve outbox events from db
            var outboxItems = await DbContext.FetchNextTransactionalOutboxEvents().ToListAsync(ct);

            if (outboxItems is null or [])
                return;

            // parse, deserialize and process events
            foreach (var outboxEvent in outboxItems)
            {
                await ProcessOutboxEvent(outboxEvent, command.Id, ct);
            }

            // save changes to outbox events
            await DbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(
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
            transactionalOutbox.ProcessedUtc = TimeProvider.GetUtcDateTime();
            Logger.LogTrace(
                "Processing events [{ProcessDomainEventsSession}] TransactionalOutboxEvent {TransactionalOutboxEvent.Id}, attempt {Attempt}",
                processingId, transactionalOutbox.Id, transactionalOutbox.Attempts);

            var eventType = Type.GetType(transactionalOutbox.EventType);
            if (eventType is null)
            {
                Logger.LogError(
                    "Processing events [{ProcessDomainEventsSession}]: Could not create DomainEvent of type `{TransactionalOutboxEvent}`.",
                    processingId, transactionalOutbox.EventType);
                return;
            }

            var domainEvent = JsonSerializer.Deserialize(transactionalOutbox.Payload, eventType);
            if (domainEvent is null)
            {
                Logger.LogError(
                    "Processing events [{ProcessDomainEventsSession}]: Could not create DomainEvent of type `{DomainEvent}`.",
                    processingId, transactionalOutbox.EventType);
                return;
            }

            await PublishOutboxEvent(domainEvent, cancellationToken);

            transactionalOutbox.Success = true;

            Logger.LogTrace(
                "Processing events [{ProcessDomainEventsSession}] TransactionalOutboxEvent {TransactionalOutboxEvent.Id}, attempt {Attempt} succeeded.",
                processingId, transactionalOutbox.Id, transactionalOutbox.Attempts);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Processing events [{ProcessDomainEventsSession}]: Could not publish DomainEvent of type `{DomainEvent}` and Id `{DomainEvent.Id}`.",
                processingId, transactionalOutbox.EventType, transactionalOutbox.Id);
            return;
        }
    }

    protected virtual async Task PublishOutboxEvent(
        object domainEvent,
        CancellationToken cancellationToken)
    {
        await Mediator.Publish(domainEvent, cancellationToken);
    }
}

