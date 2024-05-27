namespace RiverBooks.SharedKernel.DomainEvents;

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}

