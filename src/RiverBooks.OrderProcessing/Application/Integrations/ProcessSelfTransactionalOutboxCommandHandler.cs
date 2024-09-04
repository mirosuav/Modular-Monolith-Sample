using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Infrastructure.Data;
using RiverBooks.SharedKernel.TransactionalOutbox;

namespace RiverBooks.OrderProcessing.Application.Integrations;
internal class ProcessSelfTransactionalOutboxCommandHandler(
    OrderProcessingDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessSelfTransactionalOutboxCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfTransactionalOutboxCommandHandlerBase<OrderProcessingDbContext>(dbContext, publisher, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
