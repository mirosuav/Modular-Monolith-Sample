using RiverBooks.Presentation.Extensions;

namespace RiverBooks.Presentation.Auth;

public class ApiAuthorizationHandler(CircuitServicesAccessor circuitServicesAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenStore = circuitServicesAccessor.Services?.GetRequiredService<AuthenticationStore>();

        if (tokenStore is not null && tokenStore.TryGetToken(out var token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token.TokenType, token.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

