using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RiverBooks.SharedKernel;

public interface IUserClaimsProvider
{
    string? GetClaim(string claimType);
}

public class UserClaimsProvider(ClaimsPrincipal user) : IUserClaimsProvider
{
    private readonly ClaimsPrincipal _user = user;

    public string? GetClaim(string claimType)
    {
        return _user.FindFirstValue(claimType);
    }
}
