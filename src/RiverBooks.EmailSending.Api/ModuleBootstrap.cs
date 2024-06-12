using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.EmailSending.Data;
using RiverBooks.EmailSending.Domain;
using RiverBooks.EmailSending.EmailBackgroundService;
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
        string? connectionString = config.GetConnectionString("EmailSendingConnectionString");
        services.AddDbContext<EmailSendingDbContext>(options => options.UseSqlServer(connectionString));

        // Add module services
        services.AddTransient<ISendEmail, LoggingEmailSender>();
        services.AddTransient<IQueueEmailsInOutboxService, EmailOutboxRepository>();
        services.AddTransient<IGetEmailsFromOutboxService, EmailOutboxRepository>();
        services.AddTransient<IMarkEmailProcessed, EmailOutboxRepository>();
        services.AddTransient<ISendEmailsFromOutboxService, DefaultSendEmailsFromOutboxService>();

        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(IMarker).Assembly);

        // Add BackgroundWorker
        services.AddHostedService<EmailSendingBackgroundService>();

        logger.Information("{Module} module services registered", "Email Sending");
        return services;
    }
}
