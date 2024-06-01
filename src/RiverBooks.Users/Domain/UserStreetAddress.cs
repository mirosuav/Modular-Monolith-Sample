using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Users.Domain;

public class UserStreetAddress
{
    public UserStreetAddress(string userId, Address streetAddress)
    {
        UserId = Throwable.IfNullOrWhitespace(userId);
        StreetAddress = Throwable.IfNull(streetAddress);
    }

    private UserStreetAddress() { } // EF

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; } = string.Empty;
    public Address StreetAddress { get; private set; } = default!;
}

