using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.Users.Contracts;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Integrations;

internal class UserAddressIntegrationEventDispatcherHandler(
  IMediator mediator,
  ILogger<UserAddressIntegrationEventDispatcherHandler> logger) :
  INotificationHandler<AddressAddedEvent>
{
    public async Task Handle(AddressAddedEvent notification, CancellationToken ct)
    {
        var addressDetails = new UserAddressDetails(notification.NewAddress.UserId,
          notification.NewAddress.Id,
          notification.NewAddress.StreetAddress.Street1,
          notification.NewAddress.StreetAddress.Street2,
          notification.NewAddress.StreetAddress.City,
          notification.NewAddress.StreetAddress.State,
          notification.NewAddress.StreetAddress.PostalCode,
          notification.NewAddress.StreetAddress.Country);

        await mediator!.Publish(new NewUserAddressAddedIntegrationEvent(addressDetails), ct);

        logger.LogInformation("[DE Handler]New address integration event sent for {user}: {address}",
          notification.NewAddress.UserId,
          notification.NewAddress.StreetAddress);
    }
}
