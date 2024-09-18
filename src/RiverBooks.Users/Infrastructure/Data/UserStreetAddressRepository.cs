using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Infrastructure.Data;

public class UserStreetAddressRepository(UsersDbContext _dbContext) : IReadOnlyUserStreetAddressRepository
{
    public Task<UserStreetAddress?> GetById(Guid userStreetAddressId)
    {
        return _dbContext.UserStreetAddresses
            .SingleOrDefaultAsync(a => a.Id == userStreetAddressId);
    }
}