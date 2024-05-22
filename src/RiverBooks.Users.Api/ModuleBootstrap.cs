using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Infrastructure.Data;
using RiverBooks.Users.Interfaces;
using Serilog;

namespace RiverBooks.Users.Api;

public static class ModuleBootstrap
{
  public static WebApplication MapBookModuleEndpoints(this WebApplication app)
  {
    app.MapGroup("/users").MapUserEndpoints();
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

    services.AddIdentityCore<ApplicationUser>()
        .AddEntityFrameworkStores<UsersDbContext>();

    // Add User Services
    services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();
    services.AddScoped<IReadOnlyUserStreetAddressRepository, EfUserStreetAddressRepository>();

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(Users.Domain.CartItem).Assembly);

    logger.Information("{Module} module services registered", "Users");

    return services;
  }
}
