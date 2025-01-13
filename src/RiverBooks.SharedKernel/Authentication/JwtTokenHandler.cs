using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RiverBooks.SharedKernel.Helpers;
using JwtConstants = System.IdentityModel.Tokens.Jwt.JwtConstants;

namespace RiverBooks.SharedKernel.Authentication;

public class JwtTokenHandler(IConfiguration configuration, TimeProvider timeProvider) : IJwtTokenHandler
{
    public AuthToken CreateToken(string userId, string userEmailAddress)
    {
        var securityKey = CreateSecurityKey(configuration["Auth:JwtSecret"]);
        var tokenExpiration = configuration.GetValue("Auth:TokenExpirationMin", 120);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var claims = new Dictionary<string, object>
        {
            [ClaimTypes.Name] = userEmailAddress,
            [ClaimTypes.Email] = userEmailAddress,
            [ClaimTypes.NameIdentifier] = userId,
            [ClaimTypes.Sid] = SequentialGuid.NewGuid().ToString()
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Issuer = configuration["Auth:JwtIssuer"],
            Audience = configuration["Auth:JwtAudience"],
            Expires = utcNow.AddMinutes(tokenExpiration),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthToken
        {
            Token = token,
            ExpiresIn = tokenExpiration
        };
    }

    public static TokenValidationParameters CreateValidationParameters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidIssuer = configuration["Auth:JwtIssuer"],
            ValidAudience = configuration["Auth:JwtAudience"],
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = CreateSecurityKey(configuration["Auth:JwtSecret"])
        };
    }

    private static SymmetricSecurityKey CreateSecurityKey(string? secretString)
    {
        ThrowWhen.NullOrShorterThan(secretString, 32);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString));
        return securityKey;
    }
}