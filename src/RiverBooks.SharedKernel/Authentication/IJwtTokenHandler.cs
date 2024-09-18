namespace RiverBooks.SharedKernel.Authentication;

public interface IJwtTokenHandler
{
    AuthToken CreateToken(string userId, string userEmailAddress);
}