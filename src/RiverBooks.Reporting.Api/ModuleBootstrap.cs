using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Reporting.Integrations;
using Serilog;

namespace RiverBooks.Reporting.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapReportingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapReportingEndpoints();

        return app;
    }

    public static IServiceCollection AddReportingModuleServices(
          this IServiceCollection services,
          ConfigurationManager config,
          ILogger logger,
          List<System.Reflection.Assembly> mediatRAssemblies)
    {
        //var connectionString = config.GetConnectionString("OrderProcessingConnectionString");
        //services.AddDbContext<OrderProcessingDbContext>(config =>
        //  config.UseSqlServer(connectionString));

        // configure module services
        services.AddScoped<ITopSellingBooksReportService, TopSellingBooksReportService>();
        services.AddScoped<ISalesReportService, DefaultSalesReportService>();
        services.AddScoped<OrderIngestionService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(Marker).Assembly);

        logger.Information("{Module} module services registered", "Reporting");
        return services;
    }
}
