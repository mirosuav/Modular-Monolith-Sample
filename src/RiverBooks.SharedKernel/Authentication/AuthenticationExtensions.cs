using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RiverBooks.SharedKernel.Authentication;

public static class AuthenticationExtensions
{
    public static void AddJwtTokenBasedAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(static o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
         {
             o.RequireHttpsMetadata = false;
             o.SaveToken = false;
             o.TokenValidationParameters = JwtTokenHandler.CreateValidationParameters(configuration);
             // Add event handler for when authentication fails
             o.Events = new JwtBearerEvents
             {
                 OnAuthenticationFailed = context =>
                 {
                     // Log or handle the token validation failure
                     Console.WriteLine("Token validation failed: " + context.Exception.Message);

                     // Optionally modify the response
                     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                     context.Response.ContentType = "application/json";
                     var result = JsonSerializer.Serialize(new { error = "Invalid token" });
                     return context.Response.WriteAsync(result);
                 }
             };
         });
    }

    /// <summary>
    /// Make all endpoint require authenticated user by default unless AllowAnonymous is explicitly set
    /// </summary>
    public static IServiceCollection AddAuthenticatedUsersOnlyFallbackPolicy(this IServiceCollection services)
    {
        return services.AddAuthorization(authOpt =>
            authOpt.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(UserClaims.Id)
                .Build());
    }
}
