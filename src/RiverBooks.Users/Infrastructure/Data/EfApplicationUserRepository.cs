using Microsoft.EntityFrameworkCore;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;

namespace RiverBooks.Users.Infrastructure.Data;

public class EfApplicationUserRepository(UsersDbContext dbContext) : IApplicationUserRepository
{
    private readonly UsersDbContext _dbContext = dbContext;

    public Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return _dbContext.ApplicationUsers.FindAsync(userId).AsTask();
    }

    public Task<ApplicationUser?> GetUserWithAddressesByEmailAsync(string email)
    {
        return _dbContext.ApplicationUsers
          .Include(user => user.Addresses)
          .SingleOrDefaultAsync(user => user.Email == email);
    }

    public Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email)
    {
        return _dbContext.ApplicationUsers
          .Include(user => user.CartItems)
          .SingleOrDefaultAsync(user => user.Email == email);
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
