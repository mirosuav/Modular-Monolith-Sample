﻿using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.OrderProcessing.Infrastructure.Data;
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
        List<Assembly> mediatRAssemblies)
    {
        var connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");
        services.AddDbContext<OrderProcessingDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        // Materialized view for user addresses fetched from User module
        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = connectionString;
            options.SchemaName = ModuleDescriptor.Name;
            options.TableName = "UserAddressesCache";
        });

        // Add Services
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<SqlServerOrderAddressCache>();
        services.AddScoped<IOrderAddressCache, ReadThroughOrderAddressCache>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);

        return services;
    }

    public static void MigrateDatabase(
        this IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<OrderProcessingDbContext>();
        dbContext.Database.Migrate();
    }
}