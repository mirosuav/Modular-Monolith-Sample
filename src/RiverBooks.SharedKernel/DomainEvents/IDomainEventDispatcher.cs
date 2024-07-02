namespace RiverBooks.SharedKernel.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<HaveDomainEvents> entitiesWithEvents);
}

