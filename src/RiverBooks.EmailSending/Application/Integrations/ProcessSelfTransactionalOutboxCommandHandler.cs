using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Infrastructure;
using RiverBooks.SharedKernel.TransactionalOutbox;

namespace RiverBooks.EmailSending.Application.Integrations;
internal class ProcessSelfTransactionalOutboxCommandHandler(
    EmailSendingDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessSelfTransactionalOutboxCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfTransactionalOutboxCommandHandlerBase<EmailSendingDbContext>(dbContext, publisher, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
