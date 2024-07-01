using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Infrastructure.Data;

public class EfUserStreetAddressRepository(UsersDbContext _dbContext) : IReadOnlyUserStreetAddressRepository
{
    public Task<UserStreetAddress?> GetById(Guid userStreetAddressId)
    {
        return _dbContext.UserStreetAddresses
          .SingleOrDefaultAsync(a => a.Id == userStreetAddressId);
    }
}
