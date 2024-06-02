﻿using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Infrastructure;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.Users.Contracts;

namespace RiverBooks.OrderProcessing.Integrations;

internal class AddressCacheUpdatingNewUserAddressHandler(IOrderAddressCache addressCache,
  ILogger<AddressCacheUpdatingNewUserAddressHandler> logger) :
  INotificationHandler<NewUserAddressAddedIntegrationEvent>
{
    private readonly IOrderAddressCache _addressCache = addressCache;
    private readonly ILogger<AddressCacheUpdatingNewUserAddressHandler> _logger = logger;

    public async Task Handle(NewUserAddressAddedIntegrationEvent notification,
      CancellationToken cancellationToken)
    {
        var orderAddress = new OrderAddress(notification.Details.AddressId,
          new Address(notification.Details.Street1,
            notification.Details.Street2,
            notification.Details.City,
            notification.Details.State,
            notification.Details.PostalCode,
            notification.Details.Country));

        await _addressCache.StoreAsync(orderAddress, cancellationToken);

        _logger.LogInformation("Cache updated with new address {address}", orderAddress);
    }
}

