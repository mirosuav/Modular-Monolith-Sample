using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace RiverBooks.SharedKernel.Messaging.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

        var sw = Stopwatch.StartNew();

        var response = await next();

        _logger.LogInformation("Handled {RequestName} with {Response} in {ms} ms", typeof(TRequest).Name, response, sw.ElapsedMilliseconds);
        sw.Stop();
        return response;
    }
}
