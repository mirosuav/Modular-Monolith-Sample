using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Books.Infrastructure;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.Books.Application.Integrations;
internal class ProcessSelfEventsCommandHandler(
    BookDbContext dbContext,
    IMediator mediator,
    ILogger<ProcessSelfEventsCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfEventsCommandHandlerBase<BookDbContext>(dbContext, mediator, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
