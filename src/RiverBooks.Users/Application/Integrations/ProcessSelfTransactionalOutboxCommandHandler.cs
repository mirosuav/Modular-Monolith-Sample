using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.TransactionalOutbox;
using RiverBooks.Users.Infrastructure.Data;

namespace RiverBooks.Users.Application.Integrations;

internal class ProcessSelfTransactionalOutboxCommandHandler(
    UsersDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessSelfTransactionalOutboxCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfTransactionalOutboxCommandHandlerBase<UsersDbContext>(dbContext, publisher, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}

