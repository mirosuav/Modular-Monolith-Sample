using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Reporting.Infrastructure.Data;
using RiverBooks.SharedKernel.TransactionalOutbox;

namespace RiverBooks.Reporting.Application.Integrations;
internal class ProcessSelfTransactionalOutboxCommandHandler(
    ReportingDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessSelfTransactionalOutboxCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfTransactionalOutboxCommandHandlerBase<ReportingDbContext>(dbContext, publisher, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
