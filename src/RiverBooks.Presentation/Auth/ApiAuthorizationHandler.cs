using Microsoft.AspNetCore.Components.Authorization;
using RiverBooks.Presentation.Extensions;
using RiverBooks.SharedKernel.Authentication;
using System.Text.Json;

namespace RiverBooks.Presentation.Auth;

public class ApiAuthorizationHandler(IHttpContextAccessor httpContextAccessor) 
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = GetAuthenticatedUserApiToken();

        if (token is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType, token.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
    
    public AuthToken? GetAuthenticatedUserApiToken()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (!(user?.Identity?.IsAuthenticated ?? false))
            return null;
        
        var tokenJson = user.FindFirst("jwt")?.Value;

        if (tokenJson is null)
            return null;

        return JsonSerializer.Deserialize<AuthToken>(tokenJson);
    }
}

