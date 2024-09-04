using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Infrastructure;
using RiverBooks.SharedKernel.TransactionalOutbox;

namespace RiverBooks.Books.Application.Integrations;
internal class ProcessSelfTransactionalOutboxCommandHandler(
    BookDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessSelfTransactionalOutboxCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfTransactionalOutboxCommandHandlerBase<BookDbContext>(dbContext, publisher, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
