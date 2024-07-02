using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Application;

internal class LogNewAddressesHandler(ILogger<LogNewAddressesHandler> logger)
    : INotificationHandler<AddressAddedDomainEvent>
{
    public Task Handle(AddressAddedDomainEvent notification, CancellationToken ct)
    {
        logger.LogInformation("New address added to user {user}: {address}",
          notification.NewAddress.UserId,
          notification.NewAddress.StreetAddress);

        return Task.CompletedTask;
    }
}
