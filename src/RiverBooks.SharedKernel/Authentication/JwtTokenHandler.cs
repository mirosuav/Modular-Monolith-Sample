using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RiverBooks.SharedKernel.Helpers;
using System.Security.Claims;
using System.Text;

namespace RiverBooks.SharedKernel.Authentication;

public class JwtTokenHandler(IConfiguration configuration, TimeProvider timeProvider)
{
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IConfiguration _configuration = configuration;

    public static TokenValidationParameters CreateValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = CreateSecurityKey(configuration["Auth:JwtSecret"])
        };
    }

    public string CreateToken(Guid userId, string userName, string userEmailAddress)
    {
        SymmetricSecurityKey securityKey = CreateSecurityKey(_configuration["Auth:JwtSecret"]);
        int tokenExpiration = _configuration.GetValue("Auth:TokenExpirationMin", 120);

        var claims = new Dictionary<string, object>
        {
            [ClaimTypes.Name] = userName,
            [ClaimTypes.Email] = userEmailAddress,
            [ClaimTypes.NameIdentifier] = userId.ToString(),
            [ClaimTypes.Sid] = Guid.NewGuid().ToString()
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = null,
            NotBefore = _timeProvider.GetUtcNow().DateTime,
            Expires = _timeProvider.GetUtcNow().AddMinutes(tokenExpiration).DateTime,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    private static SymmetricSecurityKey CreateSecurityKey(string? secretString)
    {
        Throwable.IfNullOrWhitespace(secretString);
        Throwable.IfShorterThan(secretString, 32);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString));
        return securityKey;
    }
}



