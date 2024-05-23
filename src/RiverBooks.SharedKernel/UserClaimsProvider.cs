using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RiverBooks.SharedKernel;

public interface IUserClaimsProvider
{
    string? GetClaim(string claimType);
}

public class UserClaimsProvider : IUserClaimsProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserClaimsProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
    }
}
