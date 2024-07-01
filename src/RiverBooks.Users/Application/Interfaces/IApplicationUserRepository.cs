using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Interfaces;

public interface IApplicationUserRepository
{
    void Add(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAsync(string emailAddress);
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<ApplicationUser?> GetUserWithAddressesByEmailAsync(string email);
    Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
