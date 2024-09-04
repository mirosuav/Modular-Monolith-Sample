using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using RiverBooks.EventsProcessing.Application;

namespace RiverBooks.EventsProcessing.Api;

public static class ModuleBootstrap
{
    public static IServiceCollection AddEventsProcessingModuleServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger,
      List<Assembly> mediatRAssemblies)
    {

        // Add BackgroundWorker for processing events
        services.AddHostedService<EventsProcessingBackgroundService>();
        services.AddSingleton(TimeProvider.System);

        logger.Information("{Module} module services registered", ModuleDescriptor.Name);
        return services;
    }
}
