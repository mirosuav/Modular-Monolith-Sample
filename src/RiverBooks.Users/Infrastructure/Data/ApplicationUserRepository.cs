using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Infrastructure.Data;

public class ApplicationUserRepository(UsersDbContext dbContext) : IApplicationUserRepository
{
    public void Add(ApplicationUser user)
    {
        dbContext.ApplicationUsers.Add(user);
    }

    public Task<ApplicationUser?> GetUserByEmailAsync(string emailAddress)
    {
        return dbContext.ApplicationUsers
            .Where(u => u.Email.Equals(emailAddress))
            .SingleOrDefaultAsync();
    }

    public Task<ApplicationUser?> GetUserAsync(Guid userId)
    {
        return dbContext.ApplicationUsers.FindAsync(userId).AsTask();
    }

    public Task<ApplicationUser?> GetUserWithAddressesAsync(Guid userId)
    {
        return dbContext.ApplicationUsers
          .Include(user => user.Addresses)
          .SingleOrDefaultAsync(user => user.Id == userId);
    }

    public Task<ApplicationUser?> GetUserWithCartAsync(Guid userId)
    {
        return dbContext.ApplicationUsers
          .Include(user => user.CartItems)
          .SingleOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        var user = await dbContext.ApplicationUsers.FindAsync(userId);

        if (user is null)
            return false;

        dbContext.ApplicationUsers.Remove(user);

        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
