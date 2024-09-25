using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RiverBooks.EmailSending.Application;
using RiverBooks.EmailSending.Domain;
using RiverBooks.EmailSending.Infrastructure;

namespace RiverBooks.EmailSending.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapEmailSendingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/emails").MapEmailSendingEndpoints();

        return app;
    }

    public static IServiceCollection AddEmailSendingModule(
        this IServiceCollection services,
        ConfigurationManager config,
        Serilog.ILogger logger,
        List<Assembly> mediatRAssemblies)
    {
        // configure EF db context
        var connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");
        services.AddDbContext<EmailSendingDbContext>(options =>
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure()));

        // Add module services
        services.AddTransient<IEmailSender, LoggingEmailSender>();
        services.AddTransient<IQueueEmailsInOutboxService, EmailOutboxRepository>();
        services.AddTransient<IGetEmailsFromOutboxService, EmailOutboxRepository>();
        services.AddTransient<IMarkEmailProcessed, EmailOutboxRepository>();
        services.AddTransient<ISendEmailsFromOutboxService, DefaultSendEmailsFromOutboxService>();
        services.AddSingleton(TimeProvider.System);

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        // Add resilience policy for IEmailSender specifically
        services.AddResiliencePipeline(typeof(ISendEmailsFromOutboxService), static builder =>
        {
            // Retry policy
            builder.AddRetry(new RetryStrategyOptions
            {
                // ShouldHandle = new PredicateBuilder().Handle<Specific exception type here>()
                Delay = TimeSpan.FromSeconds(1.5),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3
            });

            // Request timeout
            builder.AddTimeout(TimeSpan.FromMinutes(1));

            // Rate limiting
            builder.AddRateLimiter(new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // max 5 requests
                Window = TimeSpan.FromSeconds(1) // per second
            }));
        });

        // Add BackgroundWorker
        services.AddHostedService<EmailSendingBackgroundService>();

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);
        return services;
    }

    public static void MigrateDatabase(
        this IServiceProvider services, ILogger logger)
    {
        var dbContext = services.GetRequiredService<EmailSendingDbContext>();
        logger.LogInformation("Migrating database for {Module}.", ModuleDescriptor.Name);
        dbContext.Database.Migrate();
    }
}