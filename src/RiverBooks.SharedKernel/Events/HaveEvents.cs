using System.ComponentModel.DataAnnotations.Schema;

namespace RiverBooks.SharedKernel.Events;

public abstract class HaveEvents
{
    [NotMapped] private readonly List<IEvent> _events = [];

    [NotMapped] public IEnumerable<IEvent> Events => _events.AsReadOnly();

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void RegisterEvent(IEvent @event)
    {
        _events.Add(@event);
    }
}