using MediatR;

namespace RiverBooks.SharedKernel.DomainEvents;

public class MediatRDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<HaveDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await publisher.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}

