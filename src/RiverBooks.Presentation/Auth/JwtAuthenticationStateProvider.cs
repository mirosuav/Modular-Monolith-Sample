using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using RiverBooks.SharedKernel.Authentication;

namespace RiverBooks.Presentation.Auth;

public class JwtAuthenticationStateProvider(
    AuthenticationStore tokenStore,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    ILogger<JwtAuthenticationStateProvider> logger) 
    : ServerAuthenticationStateProvider
{
    public async Task SignIn(HttpContext httpContext, AuthToken token)
    {
        var principal = CreateClaimsPrincipal(token);
        if (principal is null)
        {
            SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
            tokenStore.ClearToken();
            return;
        }

        SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
        await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        tokenStore.SetToken(token);
        logger.LogInformation("User {User} logged in", principal.Identity?.Name ?? "Unknown");
    }

    public async Task SignOut(HttpContext httpContext)
    {
        var user = httpContext.User.Identity?.Name;
        await httpContext.SignOutAsync();
        SetAuthenticationState(Task.FromResult(CreateEmptyAuthenticationState()));
        tokenStore.ClearToken();
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
        
        var claims = jwtToken.Claims;
        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        return new ClaimsPrincipal(identity);
    }

    private AuthenticationState CreateEmptyAuthenticationState() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}