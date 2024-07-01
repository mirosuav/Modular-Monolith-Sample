using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.Interfaces;

public interface IApplicationUserRepository
{
    void Add(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAsync(string emailAddress);
    Task<ApplicationUser?> GetUserAsync(Guid userId);
    Task<ApplicationUser?> GetUserWithAddressesAsync(Guid userId);
    Task<ApplicationUser?> GetUserWithCartAsync(Guid userId);
    Task<bool> DeleteUser(Guid userId);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
