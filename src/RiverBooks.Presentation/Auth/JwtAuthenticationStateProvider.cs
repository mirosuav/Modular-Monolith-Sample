using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel.Authentication;

namespace RiverBooks.Presentation.Auth;

public class JwtAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    ILogger<JwtAuthenticationStateProvider> logger) 
    : ServerAuthenticationStateProvider
{
    public async Task SignIn(AuthToken token)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new ApplicationException("HttpContext is not available");

        var principal = CreateClaimsPrincipal(token);
        if (principal is null)
        {
            SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
            return;
        }

        SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
        
        await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        logger.LogInformation("User {User} logged in", principal.Identity?.Name ?? "Unknown");
    }

    public async Task SignOut()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            throw new ApplicationException("HttpContext is not available");
        
        var user = httpContext.User.Identity?.Name;
        await httpContext.SignOutAsync();
        SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
        if (user != null)
            logger.LogInformation("User {User} logged out", user);
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