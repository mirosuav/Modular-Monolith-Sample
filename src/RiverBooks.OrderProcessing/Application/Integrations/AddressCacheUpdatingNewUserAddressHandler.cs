using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Application.Integrations;

internal class AddressCacheUpdatingNewUserAddressHandler(
    IOrderAddressCache addressCache,
    ILogger<AddressCacheUpdatingNewUserAddressHandler> logger) :
    INotificationHandler<NewUserAddressAddedIntegrationEvent>
{
    public async Task Handle(NewUserAddressAddedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        var orderAddress = new OrderAddress(notification.UserAddress.AddressId,
            new Address(notification.UserAddress.Street1,
                notification.UserAddress.Street2,
                notification.UserAddress.City,
                notification.UserAddress.State,
                notification.UserAddress.PostalCode,
                notification.UserAddress.Country));

        await addressCache.StoreAsync(orderAddress, cancellationToken);

        logger.LogInformation("Cache updated with new address {address}", orderAddress);
    }
}