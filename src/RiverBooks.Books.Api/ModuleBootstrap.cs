using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Books.Api;
using RiverBooks.Books.Data;

namespace RiverBooks.Books;

public static class ModuleBootstrap
{
    public static WebApplication MapBookModuleEndpoints(this WebApplication app)
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
        string? connectionString = config.GetConnectionString("BooksConnectionString");
        services.AddDbContext<BookDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IBookRepository, EfBookRepository>();
        services.AddScoped<IBookService, BookService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(BookService).Assembly);

        logger.Information("{Module} module services registered", "Books");

        return services;
    }
}
