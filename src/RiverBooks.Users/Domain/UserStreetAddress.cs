using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Helpers;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Domain;

public class UserStreetAddress
{
    public UserStreetAddress(Guid userId, Address streetAddress)
    {
        UserId = PassOrThrowWhen.Empty(userId);
        StreetAddress = PassOrThrowWhen.Null(streetAddress);
    }

    private UserStreetAddress()
    {
        // EF
    }

    public Guid Id { get; } = SequentialGuid.NewGuid();

    public Guid UserId { get; private set; }

    public Address StreetAddress { get; private set; } = default!;

    public UserAddressDto ToDto()
    {
        return new UserAddressDto(
            UserId,
            Id,
            StreetAddress.Street1,
            StreetAddress.Street2,
            StreetAddress.City,
            StreetAddress.State,
            StreetAddress.PostalCode,
            StreetAddress.Country);
    }
}