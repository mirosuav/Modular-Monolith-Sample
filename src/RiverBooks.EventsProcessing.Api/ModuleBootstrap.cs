using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.EventsProcessing.Application;

namespace RiverBooks.EventsProcessing.Api;

public static class ModuleBootstrap
{
    public static IServiceCollection AddEventsProcessingModule(
        this IServiceCollection services,
        ConfigurationManager config,
        List<Assembly> mediatRAssemblies)
    {
        // Add BackgroundWorker for processing events
        services.AddHostedService<EventsProcessingBackgroundService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}