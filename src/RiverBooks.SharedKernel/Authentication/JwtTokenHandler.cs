using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RiverBooks.SharedKernel.Helpers;
using System.Text;

namespace RiverBooks.SharedKernel.Authentication;

public class JwtTokenHandler(IConfiguration configuration, TimeProvider timeProvider) : IJwtTokenHandler
{
    public static TokenValidationParameters CreateValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = CreateSecurityKey(configuration["Auth:JwtSecret"])
        };
    }

    public string CreateToken(string userId, string userEmailAddress)
    {
        SymmetricSecurityKey securityKey = CreateSecurityKey(configuration["Auth:JwtSecret"]);
        int tokenExpiration = configuration.GetValue("Auth:TokenExpirationMin", 120);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var claims = new Dictionary<string, object>
        {
            [UserClaims.Email] = userEmailAddress,
            [UserClaims.Id] = userId,
            [UserClaims.TokenId] = SequentialGuid.NewGuid().ToString()
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Expires = utcNow.AddMinutes(tokenExpiration),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    private static SymmetricSecurityKey CreateSecurityKey(string? secretString)
    {
        ThrowWhen.NullOrShorterThan(secretString, 32);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString));
        return securityKey;
    }
}



