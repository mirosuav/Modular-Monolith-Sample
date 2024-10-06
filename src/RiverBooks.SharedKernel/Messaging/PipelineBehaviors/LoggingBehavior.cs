using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RiverBooks.SharedKernel.Messaging.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!logger.IsEnabled(LogLevel.Debug))
            return await next();

        logger.LogDebug("Handling {RequestName}", typeof(TRequest).Name);

        var sw = Stopwatch.StartNew();

        var response = await next();

        logger.LogDebug("Handled {RequestName} with {ResponseType} in {ms} ms",
            typeof(TRequest).Name,
            response?.GetType().Name,
            sw.ElapsedMilliseconds);

        sw.Stop();

        return response;
    }
}