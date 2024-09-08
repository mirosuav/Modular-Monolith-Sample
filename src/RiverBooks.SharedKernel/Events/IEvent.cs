using MediatR;

namespace RiverBooks.SharedKernel.Events;

public interface IEvent : INotification
{
    Guid Id { get; set; }
    DateTimeOffset OccurredUtc { get; init; }
}