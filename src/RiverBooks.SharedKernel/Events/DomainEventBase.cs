namespace RiverBooks.SharedKernel.Events;

public abstract class DomainEventBase(DateTime occuredUtc) : IEvent
{
    public Guid Id { get; set; } = SequentialGuid.NewGuid();
    public DateTime OccurredUtc { get; init; } = occuredUtc;
}

