using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class UserStreetAddress
{
    public UserStreetAddress(string userId, Address streetAddress)
    {
        UserId = PassOrThrow.IfNullOrWhitespace(userId);
        StreetAddress = PassOrThrow.IfNull(streetAddress);
    }

    private UserStreetAddress() { } // EF

    public Guid Id { get; private set; } = SequentialGuid.NewGuid();
    public string UserId { get; private set; } = string.Empty;
    public Address StreetAddress { get; private set; } = default!;
}

