using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Users.Domain;

internal sealed class AddressAddedDomainEvent(UserStreetAddress newAddress, DateTime occuredUtc) 
    : DomainEventBase(occuredUtc)
{
    public UserStreetAddress NewAddress { get; } = newAddress;
}
