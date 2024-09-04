using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Application;
using RiverBooks.Books.Infrastructure;

namespace RiverBooks.Books.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapBookModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/books").MapBookEndpoints();
        return app;
    }

    public static IServiceCollection AddBookModuleServices(
      this IServiceCollection services,
      ConfigurationManager config,
      Serilog.ILogger logger,
      List<System.Reflection.Assembly> mediatRAssemblies)
    {
        string? connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");
        services.AddDbContext<BookDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddSingleton(TimeProvider.System);

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);

        return services;
    }
}
