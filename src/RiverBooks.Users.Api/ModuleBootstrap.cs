using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.SharedKernel.Messaging.PipelineBehaviors;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Infrastructure.Data;
using RiverBooks.Users.Interfaces;
using RiverBooks.Users.UseCases.Cart.AddItem;
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
      List<System.Reflection.Assembly> mediatRAssemblies)
    {
        string? connectionString = config.GetConnectionString("UsersConnectionString");

        services.AddDbContext<UsersDbContext>(config => config.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<UsersDbContext>();

        // Add User Services
        services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();
        services.AddScoped<IReadOnlyUserStreetAddressRepository, EfUserStreetAddressRepository>();

        services.AddValidatorsFromAssemblyContaining<Marker>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(Marker).Assembly);

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}
