using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using RiverBooks.EmailSending.Application;
using RiverBooks.EmailSending.Domain;
using RiverBooks.EmailSending.Infrastructure;
using Serilog;
using System.Reflection;

namespace RiverBooks.EmailSending.Api;

public static class ModuleBootstrap
{
    public static IEndpointRouteBuilder MapEmailSendingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/emails").MapEmailSendingEndpoints();

        return app;
    }

    public static IServiceCollection AddEmailSendingModuleServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger,
      List<Assembly> mediatRAssemblies)
    {

        // configure EF db context
        string? connectionString = config.GetConnectionString($"{ModuleDescriptor.Name}ConnectionString");
        services.AddDbContext<EmailSendingDbContext>(options => options.UseSqlServer(connectionString));

        // Add module services
        services.AddTransient<IEmailSender, LoggingEmailSender>();
        services.AddTransient<IQueueEmailsInOutboxService, EmailOutboxRepository>();
        services.AddTransient<IGetEmailsFromOutboxService, EmailOutboxRepository>();
        services.AddTransient<IMarkEmailProcessed, EmailOutboxRepository>();
        services.AddTransient<ISendEmailsFromOutboxService, DefaultSendEmailsFromOutboxService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(ModuleDescriptor).Assembly);

        services.AddResiliencePipeline(typeof(IEmailSender), static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                Delay = TimeSpan.FromSeconds(1.5),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3
            });

            builder.AddTimeout(TimeSpan.FromSeconds(15));
        });

        // Add BackgroundWorker
        services.AddHostedService<EmailSendingBackgroundService>();

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);
        return services;
    }
}
