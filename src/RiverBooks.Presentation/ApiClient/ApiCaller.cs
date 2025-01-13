using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RiverBooks.SharedKernel.Authentication;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.Presentation.ApiClient;

public class ApiAuthenticator
{
    private readonly HttpClient _httpClient;
    private readonly HttpContextAccessor _httpContextAccessor;
    private ILogger<ApiAuthenticator> _logger;

    public ApiAuthenticator(ILogger<ApiAuthenticator> logger, HttpClient httpClient,
        HttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResultOf> RegisterNewUser(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/users", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("RegisterNewUser failed with status code {RegisterUserStatusCode}", response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Validation(details?.Title ?? "User registration failed", details?.Detail ?? "UnexpectedError");
        }

        return ResultOf.Success();
    }

    public async Task<ResultOf> LoginUser(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/users/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("RegisterNewUser failed with status code {RegisterUserStatusCode}", response.StatusCode);
            var details = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return Error.Unauthorized(details?.Title ?? "User login failed");
        }

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();

        if (string.IsNullOrWhiteSpace(authToken.Token) || _httpContextAccessor.HttpContext is null)
            return Error.ServerError;

        var userIdentity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, authToken.Token));
        userIdentity.AddClaim(new Claim(ClaimTypes.Name, email));
        userIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
        var userPrincipal = new ClaimsPrincipal(userIdentity);

        await _httpContextAccessor.HttpContext
            .SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal,
                new AuthenticationProperties());

        _httpContextAccessor.HttpContext.User = userPrincipal;

        return ResultOf.Success();
    }
}