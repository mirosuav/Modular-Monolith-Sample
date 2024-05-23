using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using RiverBooks.OrderProcessing.Interfaces;
using Serilog;

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
    List<System.Reflection.Assembly> mediatRAssemblies)
    {
        string? connectionString = config.GetConnectionString("OrderProcessingConnectionString");
        services.AddDbContext<OrderProcessingDbContext>(config =>
          config.UseSqlServer(connectionString));

        // Add Services
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<RedisOrderAddressCache>();
        services.AddScoped<IOrderAddressCache, ReadThroughOrderAddressCache>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleBootstrap).Assembly);

        logger.Information("{Module} module services registered", "OrderProcessing");

        return services;
    }
}
