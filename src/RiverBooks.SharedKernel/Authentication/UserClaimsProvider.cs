using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RiverBooks.SharedKernel.Authentication;

public class UserClaimsProvider(IHttpContextAccessor httpContextAccessor) : IUserClaimsProvider
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public string? GetClaim(string claimType)
    {
        return _user?.FindFirstValue(claimType);
    }

    public string? GetEmailAddress() =>
        GetClaim(UserClaims.Email);

    public string? GetId() =>
        GetClaim(UserClaims.Id);
}
