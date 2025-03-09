using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Infrastructure.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RiverBooks.Reporting.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapReportingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/reports")
            .MapReportingEndpoints();

        return app;
    }

    public static IServiceCollection AddReportingModule(
        this IServiceCollection services,
        ConfigurationManager config,
        List<Assembly> mediatRAssemblies)
    {
        var connectionString = config.GetConnectionString("riverbooksdb");

        services.AddDbContext<ReportingDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        // configure module services
        services.AddScoped<ISalesReportService, SalesReportService>();
        services.AddScoped<ISalesReportRepository, SalesReportRepository>();
        services.AddSingleton(TimeProvider.System);

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        return services;
    }

    public static void MigrateDatabase(
        this IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<ReportingDbContext>();
        logger.LogInformation("Migrating database for {Module}.", ModuleDescriptor.Name);
        dbContext.Database.Migrate();
    }
}