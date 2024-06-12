
namespace RiverBooks.SharedKernel.Authentication
{
    public interface IJwtTokenHandler
    {
        string CreateToken(string userId, string userEmailAddress);
    }
}