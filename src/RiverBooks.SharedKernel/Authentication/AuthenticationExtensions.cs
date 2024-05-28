﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
             o.TimeProvider = TimeProvider.System;
             o.RequireHttpsMetadata = false;
             o.SaveToken = false;
             o.TokenValidationParameters = JwtTokenHandler.CreateValidationParameters(configuration);
         });
    }

    public static void AddAuthenticatedUsersOnlyFallbackPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(authOpt =>
            authOpt.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
    }
}