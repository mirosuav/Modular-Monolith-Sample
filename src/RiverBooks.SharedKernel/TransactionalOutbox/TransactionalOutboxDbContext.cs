using Microsoft.EntityFrameworkCore;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.SharedKernel.TransactionalOutbox;

/// <summary>
/// Implementation of Transactional Outbox pattern
/// DomainEvents are persisted with atomic fashion within the main business entity
/// Then EventsProcessing module triggers the events from outbox table in each individual module asynchronously
/// </summary>
public abstract class TransactionalOutboxDbContext(DbContextOptions options)
    : DbContext(options)
{
    protected static readonly int TransactionalOutboxMaxAttempts = 3;

    protected static readonly Func<TransactionalOutboxDbContext, IQueryable<TransactionalOutboxEvent>>
        FetchNextTransactionalOutboxEventsQuery =
        EF.CompileQuery((TransactionalOutboxDbContext context) =>
                context
                .OutboxEvents
                .Where(x => !x.Success) // Get not completed
                .Where(x => x.Attempts < TransactionalOutboxMaxAttempts) // Exclude persistent failures
                .OrderBy(x => x.OccurredUtc) // Order by FiFo
        );

    public IQueryable<TransactionalOutboxEvent> FetchNextTransactionalOutboxEvents() =>
        FetchNextTransactionalOutboxEventsQuery.Invoke(this);

    public DbSet<TransactionalOutboxEvent> OutboxEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add filtered index to improve the performance
        modelBuilder.Entity<TransactionalOutboxEvent>()
            .HasIndex(e => e.OccurredUtc)
            .HasFilter($"[Success] = 0 AND [Attempts] < {TransactionalOutboxMaxAttempts}");

        modelBuilder.Entity<TransactionalOutboxEvent>()
            .Property(x => x.Id)
            .ValueGeneratedNever();
    }

    /// <summary>
    /// Retrieve domain events from all entities and save them in the EventsOutbox
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StoreOutboxEvents();

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private void StoreOutboxEvents()
    {
        // capture domain events and store them as OutboxEvents
        foreach (var entry in ChangeTracker.Entries<HaveEvents>())
        {
            var outboxEvents = entry.Entity
                .DomainEvents
                .Select(TransactionalOutboxEvent.Create)
                .ToArray();

            OutboxEvents.AddRange(outboxEvents);

            entry.Entity.ClearDomainEvents();
        }
    }
}

