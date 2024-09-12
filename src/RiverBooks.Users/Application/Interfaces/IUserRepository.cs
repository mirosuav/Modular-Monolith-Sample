using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application.Interfaces;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetUserByEmailAsync(string emailAddress);
    Task<User?> GetUserAsync(Guid userId);
    Task<User?> GetUserWithAddressesAsync(Guid userId);
    Task<User?> GetUserWithCartAsync(Guid userId);
    Task<bool> DeleteUser(Guid userId);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
