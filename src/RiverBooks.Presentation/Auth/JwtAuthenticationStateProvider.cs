using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace RiverBooks.Presentation.Auth;

public class JwtAuthenticationStateProvider(
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    ILogger<JwtAuthenticationStateProvider> logger)
    : ServerAuthenticationStateProvider
{
    public Task SignIn(AuthToken token)
    {
        var principal = CreateClaimsPrincipal(token);
        if (principal is null)
        {
            SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
            return Task.CompletedTask;
        }

        SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));

        logger.LogInformation("User {User} logged in", principal.Identity?.Name ?? "Unknown");
        return Task.CompletedTask;
    }

    public Task SignOut()
    {
        SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
        logger.LogInformation("User logged out");
        return Task.CompletedTask;
    }

    private ClaimsPrincipal? CreateClaimsPrincipal(AuthToken token)
    {
        var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(token.Token);

        if (jwtToken.ValidTo < DateTime.UtcNow)
        {
            return null;
        }

        var claims = jwtToken.Claims.ToList();
        claims.Add(new Claim("jwt", JsonSerializer.Serialize(token)));
        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        return new ClaimsPrincipal(identity);
    }

    private AuthenticationState CreateEmptyAuthenticationState() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}