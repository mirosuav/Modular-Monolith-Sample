using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Reporting.Application;
using RiverBooks.Reporting.Infrastructure.Data;
using Serilog;

namespace RiverBooks.Reporting.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapReportingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/reports")
            .MapReportingEndpoints();

        return app;
    }

    public static IServiceCollection AddReportingModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<Assembly> mediatRAssemblies)
    {
        var connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");

        services.AddDbContext<ReportingDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        // configure module services
        services.AddScoped<ISalesReportService, SalesReportService>();
        services.AddScoped<ISalesReportRepository, SalesReportRepository>();
        services.AddSingleton(TimeProvider.System);

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);
        return services;
    }

    public static void MigrateDatabase(
        this IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<ReportingDbContext>();
        dbContext.Database.Migrate();
    }
}