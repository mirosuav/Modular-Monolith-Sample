using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RiverBooks.SharedKernel.Events;

/// <summary>
///     Holds a DomainEvent in Transactional Outbox table
/// </summary>
public sealed class TransactionalOutboxEvent
{
    public Guid Id { get; init; }
    public DateTimeOffset OccurredUtc { get; init; }
    public DateTimeOffset? ProcessedUtc { get; set; }
    public bool Success { get; set; }
    public int Attempts { get; set; }

    [StringLength(300)] public required string EventType { get; init; }

    /// <summary>
    ///     Json serialized DomainEvent
    /// </summary>
    [StringLength(2000)]
    public required string Payload { get; init; }

    public static TransactionalOutboxEvent Create(IEvent domainEvent)
    {
        return new TransactionalOutboxEvent
        {
            Id = domainEvent.Id,
            OccurredUtc = domainEvent.OccurredUtc,
            EventType = domainEvent.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
        };
    }
}