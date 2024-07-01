using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using Serilog;
using System.Reflection;

namespace RiverBooks.OrderProcessing.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapOrderProcessingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/orders").MapOrderProcessingEndpoints();

        return app;
    }

    public static IServiceCollection AddOrderProcessingModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<Assembly> mediatRAssemblies)
    {
        string? connectionString = config.GetConnectionString("OrderProcessingConnectionString");
        services.AddDbContext<OrderProcessingDbContext>(config => config.UseSqlServer(connectionString));

        // Materialized view for user addresses fetched from User module
        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = "OrderProcessing";
            options.TableName = "UserAdressesCache";
        });

        // Add Services
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<SqlServerOrderAddressCache>();
        services.AddScoped<IOrderAddressCache, ReadThroughOrderAddressCache>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(IMarker).Assembly);

        logger.Information("{Module} module services registered", "OrderProcessing");

        return services;
    }
}
