using RiverBooks.SharedKernel.DomainEvents;

namespace RiverBooks.Users.Domain;

internal sealed class AddressAddedEvent : DomainEventBase
{
    public AddressAddedEvent(UserStreetAddress newAddress)
    {
        NewAddress = newAddress;
    }

    public UserStreetAddress NewAddress { get; }
}
