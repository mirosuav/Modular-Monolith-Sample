namespace RiverBooks.SharedKernel.Events;

public abstract record EventBase(DateTimeOffset OccurredUtc) : IEvent
{
    public Guid Id { get; set; } = SequentialGuid.NewGuid();
    public DateTimeOffset OccurredUtc { get; init; } = OccurredUtc;
}