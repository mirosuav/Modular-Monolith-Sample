﻿using MediatR;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal class ReadThroughOrderAddressCache(
    SqlServerOrderAddressCache addressCache,
    IMediator mediator,
    ILogger<ReadThroughOrderAddressCache> logger) : IOrderAddressCache
{
    private readonly SqlServerOrderAddressCache _addressCache = addressCache;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ReadThroughOrderAddressCache> _logger = logger;
    private static readonly SemaphoreSlim cacheAccessMonitor = new(1, 1);

    public async Task<Resultable<OrderAddress>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Read user address from cache
        await cacheAccessMonitor.WaitAsync(cancellationToken);

        try
        {
            var result = await _addressCache.GetByIdAsync(id, cancellationToken);
            if (result.IsSuccess)
                return result;

            // read user address from User module and store in cache
            _logger.LogInformation("Address {id} not found; fetching from source.", id);
            var query = new Users.Contracts.UserAddressDetailsByIdQuery(id);

            var queryResult = await _mediator.Send(query, cancellationToken);

            if (queryResult.IsSuccess)
            {
                var dto = queryResult.Value;
                var address = new Address(dto.Street1,
                                          dto.Street2,
                                          dto.City,
                                          dto.State,
                                          dto.PostalCode,
                                          dto.Country);

                var orderAddress = new OrderAddress(dto.AddressId, address);
                await _addressCache.StoreAsync(orderAddress, cancellationToken);
                return orderAddress;
            }

            return Error.NotFound("Could not retreive user address");
        }
        finally
        {
            cacheAccessMonitor.Release();
        }
    }

    public async Task<Resultable> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken)
    {
        await cacheAccessMonitor.WaitAsync(cancellationToken);
        try
        {
            return await _addressCache.StoreAsync(orderAddress, cancellationToken);
        }
        finally
        {
            cacheAccessMonitor.Release();
        }
    }
}
