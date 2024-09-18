using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.SharedKernel.Events;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Infrastructure.Data;

namespace RiverBooks.Users.Application.Integrations;

internal class ProcessSelfEventsCommandHandler(
    UsersDbContext dbContext,
    IMediator mediator,
    ILogger<ProcessSelfEventsCommandHandler> logger,
    TimeProvider timeProvider) :
    ProcessSelfEventsCommandHandlerBase<UsersDbContext>(dbContext, mediator, logger, timeProvider)
{
    private static readonly SemaphoreSlim _accessLocker = new(1, 1);
    protected override SemaphoreSlim AccessLocker => _accessLocker;

    protected override async Task PublishOutboxEvent(object domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is AddressAddedEvent addressAddedEvent)
        {
            var address = await DbContext.UserStreetAddresses
                .FindAsync(addressAddedEvent.UserAddressId, cancellationToken);

            if (address is null)
                throw new ApplicationException($"Could not find User address [{addressAddedEvent.UserAddressId}].");

            await Mediator.Publish(new NewUserAddressAddedIntegrationEvent(address.ToDto()), cancellationToken);
        }
        else
        {
            await base.PublishOutboxEvent(domainEvent, cancellationToken);
        }
    }
}