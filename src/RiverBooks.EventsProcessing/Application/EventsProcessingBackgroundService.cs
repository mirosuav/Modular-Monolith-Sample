using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel;
using RiverBooks.SharedKernel.Events;
using RiverBooks.SharedKernel.Extensions;

namespace RiverBooks.EventsProcessing.Application;

[DebuggerStepThrough]
internal class EventsProcessingBackgroundService(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<EventsProcessingBackgroundService> logger) : BackgroundService
{
    // Process module events interval in ms
    private const int ProcessModuleEventsInterval = 2_000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{ServiceName} started.", ModuleDescriptor.Name);
        using var scope = scopeFactory.CreateScope();

        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var sessionId = SequentialGuid.NewGuid();
            try
            {
                var processingSession = new ProcessSelfEventsCommand(
                    sessionId,
                    timeProvider.GetUtcDateTime());

                logger.LogTrace(
                    "Dispatching ProcessSelfEventsCommand [session={ProcessDomainEventsSession}] at {ProcessDomainEventsSessionTimeStamp} ",
                    processingSession.Id, processingSession.CreatedUtc);

                // Sends command to all modules using common mediator publisher
                await publisher.Publish(processingSession, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing events [session={ProcessDomainEventsSession}]: {ErrorMessage}",
                    sessionId, ex.Message);
            }
            finally
            {
                logger.LogTrace("{ServiceName} is sleeping {ProcessModuleEventsInterval}ms...",
                    nameof(EventsProcessingBackgroundService),
                    ProcessModuleEventsInterval);

                await Task.Delay(ProcessModuleEventsInterval, stoppingToken);
            }
        }

        logger.LogInformation("{ServiceName} exited.", ModuleDescriptor.Name);
    }
}