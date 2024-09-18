namespace RiverBooks.Users.Contracts;

public record UserAddressDto(
    Guid UserId,
    Guid AddressId,
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);