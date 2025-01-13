using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RiverBooks.SharedKernel.Authentication;

public class UserClaimsProvider(IHttpContextAccessor httpContextAccessor) : IUserClaimsProvider
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public string? GetClaim(string claimType)
    {
        return _user?.FindFirstValue(claimType);
    }

    public string? GetEmailAddress()
    {
        return GetClaim(ClaimTypes.Email);
    }

    public Guid? GetId()
    {
        return Guid.TryParse(GetClaim(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }
}