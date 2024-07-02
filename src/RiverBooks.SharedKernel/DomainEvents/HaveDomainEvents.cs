using System.ComponentModel.DataAnnotations.Schema;

namespace RiverBooks.SharedKernel.DomainEvents;

public abstract class HaveDomainEvents
{    
    private readonly List<DomainEventBase> _domainEvents = [];

    [NotMapped]
    public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

}

