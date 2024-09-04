using MediatR;

namespace RiverBooks.SharedKernel.Events;

public interface IEvent : INotification
{
    Guid Id { get; set; }
    DateTime OccurredUtc { get; init; }
}