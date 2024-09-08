using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Reporting.Infrastructure.Data;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Reporting.Application.Integrations;
internal class ProcessSelfEventsCommandHandler(
    ReportingDbContext dbContext,
    IMediator mediator,
    ILogger<ProcessSelfEventsCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfEventsCommandHandlerBase<ReportingDbContext>(dbContext, mediator, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
