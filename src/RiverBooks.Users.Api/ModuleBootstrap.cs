using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.Users.Application.Interfaces;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Infrastructure.Data;
using Serilog;

namespace RiverBooks.Users.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapUserModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/users").MapUserEndpoints();
        app.MapGroup("/cart").MapCartEndpoints();

        return app;
    }

    public static IServiceCollection AddUserModuleServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger,
      List<System.Reflection.Assembly> modulesAssemblies)
    {
        string? connectionString = config.GetConnectionString("UsersConnectionString");

        services.AddDbContext<UsersDbContext>(c => c.UseSqlServer(connectionString));
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
}
