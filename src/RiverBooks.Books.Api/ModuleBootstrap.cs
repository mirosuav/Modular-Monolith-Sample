using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Application;
using RiverBooks.Books.Infrastructure;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Books.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapBookModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/books").MapBookEndpoints();
        return app;
    }

    public static IServiceCollection AddBooksModule(
        this IServiceCollection services,
        ConfigurationManager config,
        Serilog.ILogger logger,
        List<Assembly> mediatRAssemblies)
    {
        var connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");
        services.AddDbContext<BookDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddSingleton(TimeProvider.System);

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);

        return services;
    }

    public static void MigrateDatabase(
        this IServiceProvider services,
        ILogger logger)
    {
        var dbContext = services.GetRequiredService<BookDbContext>();
        logger.LogInformation("Migrating database for {Module}.", ModuleDescriptor.Name);
        dbContext.Database.Migrate();
    }
}