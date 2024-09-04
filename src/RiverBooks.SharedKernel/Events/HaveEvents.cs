using System.ComponentModel.DataAnnotations.Schema;

namespace RiverBooks.SharedKernel.Events;

public abstract class HaveEvents
{
    [NotMapped]
    private readonly List<IEvent> _domainEvents = [];

    [NotMapped]
    public IEnumerable<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RegisterDomainEvent(IEvent domainEvent) => _domainEvents.Add(domainEvent);

}

