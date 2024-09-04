using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.SharedKernel.TransactionalOutbox;

/// <summary>
/// Holds a DomainEvent in Transactional Outbox table
/// </summary>
public sealed class TransactionalOutboxEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredUtc { get; init; }
    public DateTime? ProcessedUtc { get; set; }
    public bool Success { get; set; }
    public int Attempts { get; set; }

    [StringLength(300)]
    public required string EventType { get; init; }

    /// <summary>
    /// Json serialized DomainEvent
    /// </summary>
    [StringLength(2000)]
    public required string Payload { get; init; }

    public static TransactionalOutboxEvent Create<TEvent>(TEvent domainEvent)
        where TEvent : class, IEvent =>
         new()
         {
             Id = domainEvent.Id,
             OccurredUtc = domainEvent.OccurredUtc,
             EventType = typeof(TEvent).Name,
             Payload = JsonSerializer.Serialize(domainEvent),
         };
}

