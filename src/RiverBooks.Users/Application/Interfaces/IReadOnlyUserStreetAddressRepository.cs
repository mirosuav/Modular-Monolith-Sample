using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.Interfaces;

public interface IReadOnlyUserStreetAddressRepository
{
    Task<UserStreetAddress?> GetById(Guid userStreetAddressId);
}