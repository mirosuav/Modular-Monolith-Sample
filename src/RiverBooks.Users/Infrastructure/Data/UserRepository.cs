using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Infrastructure.Data;

public class UserRepository(UsersDbContext dbContext) : IApplicationUserRepository
{
    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }

    public Task<User?> GetUserByEmailAsync(string emailAddress)
    {
        return dbContext.Users
            .Where(u => u.Email.Equals(emailAddress))
            .SingleOrDefaultAsync();
    }

    public Task<User?> GetUserAsync(Guid userId)
    {
        return dbContext.Users.FindAsync(userId).AsTask();
    }

    public Task<User?> GetUserWithAddressesAsync(Guid userId)
    {
        return dbContext.Users
          .Include(user => user.Addresses)
          .SingleOrDefaultAsync(user => user.Id == userId);
    }

    public Task<User?> GetUserWithCartAsync(Guid userId)
    {
        return dbContext.Users
          .Include(user => user.CartItems)
          .SingleOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user is null)
            return false;

        dbContext.Users.Remove(user);

        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
