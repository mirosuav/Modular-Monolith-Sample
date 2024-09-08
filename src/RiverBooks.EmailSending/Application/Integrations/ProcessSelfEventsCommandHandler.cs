using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.EmailSending.Infrastructure;
using RiverBooks.SharedKernel.Events;

namespace RiverBooks.EmailSending.Application.Integrations;
internal class ProcessSelfEventsCommandHandler(
    EmailSendingDbContext dbContext,
    IMediator mediator,
    ILogger<ProcessSelfEventsCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfEventsCommandHandlerBase<EmailSendingDbContext>(dbContext, mediator, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;
}
