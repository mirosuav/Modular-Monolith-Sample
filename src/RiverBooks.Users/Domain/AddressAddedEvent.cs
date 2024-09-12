using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Users.Domain;

internal sealed record AddressAddedEvent(Guid UserAddressId, DateTimeOffset OccurredUtc) : EventBase(OccurredUtc);
