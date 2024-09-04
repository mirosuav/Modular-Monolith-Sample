namespace RiverBooks.SharedKernel.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<HaveEvents> entitiesWithEvents);
}

