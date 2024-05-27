using MediatR;

namespace RiverBooks.SharedKernel.DomainEvents;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await _publisher.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}

