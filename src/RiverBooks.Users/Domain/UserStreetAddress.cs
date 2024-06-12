using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class UserStreetAddress
{
    public UserStreetAddress(Guid userId, Address streetAddress)
    {
        UserId = PassOrThrow.IfEmpty(userId);
        StreetAddress = PassOrThrow.IfNull(streetAddress);
    }

    private UserStreetAddress() { } // EF

    public Guid Id { get; private set; } = SequentialGuid.NewGuid();
    public Guid UserId { get; private set; } = SequentialGuid.Empty;
    public Address StreetAddress { get; private set; } = default!;
}

