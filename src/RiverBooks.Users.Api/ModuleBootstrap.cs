using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Infrastructure.Data;

namespace RiverBooks.Users.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapUserModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/users").MapUserEndpoints();
        app.MapGroup("/cart").MapCartEndpoints();

        return app;
    }

    public static IServiceCollection AddUserModule(
        this IServiceCollection services,
        ConfigurationManager config,
        Serilog.ILogger logger,
        List<Assembly> modulesAssemblies)
    {
        var connectionString = config.GetConnectionString("UsersConnectionString");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        services.AddSingleton(TimeProvider.System);

        // Add User Services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReadOnlyUserStreetAddressRepository, UserStreetAddressRepository>();

        services.AddValidatorsFromAssemblyContaining<ModuleDescriptor>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        modulesAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);

        return services;
    }

    public static void MigrateDatabase(this IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<UsersDbContext>();
        logger.LogInformation("Migrating database for {Module}.", ModuleDescriptor.Name);
        dbContext.Database.Migrate();
    }
}