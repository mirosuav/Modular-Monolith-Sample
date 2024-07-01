using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Infrastructure.Data;

public class EfApplicationUserRepository(UsersDbContext dbContext) : IApplicationUserRepository
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

    public Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return dbContext.ApplicationUsers.FindAsync(userId).AsTask();
    }

    public Task<ApplicationUser?> GetUserWithAddressesByEmailAsync(string email)
    {
        return dbContext.ApplicationUsers
          .Include(user => user.Addresses)
          .SingleOrDefaultAsync(user => user.Email == email);
    }

    public Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email)
    {
        return dbContext.ApplicationUsers
          .Include(user => user.CartItems)
          .SingleOrDefaultAsync(user => user.Email == email);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
