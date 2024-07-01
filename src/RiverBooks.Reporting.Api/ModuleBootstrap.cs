using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Infrastructure;
using Serilog;
using System.Reflection;

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
          List<Assembly> mediatRAssemblies)
    {
        // configure module services
        services.AddScoped<ITopSellingBooksReportService, TopSellingBooksReportService>();
        services.AddScoped<ISalesReportService, DefaultSalesReportService>();
        services.AddScoped<OrderIngestionService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(IMarker).Assembly);

        logger.Information("{Module} module services registered", "Reporting");
        return services;
    }
}
