using MediatR;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
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
    private readonly AsyncLock cacheAccessMonitor = new();

    public async Task<Resultable<OrderAddress>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Read user address from cache
        var result = await _addressCache.GetByIdAsync(id, cancellationToken);
        if (result.IsSuccess)
            return result;

        using (await cacheAccessMonitor.LockAsync(cancellationToken))
        {
            result = await _addressCache.GetByIdAsync(id, cancellationToken);
            if (result.IsSuccess)
                return result;

            // read user address from User module and store in cache
            _logger.LogInformation("Address {id} not found; fetching from source.", id);
            var query = new Users.Contracts.UserAddressDetailsByIdQuery(id);

            var queryResult = await _mediator.Send(query);

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
                await StoreAsync(orderAddress, cancellationToken);
                return orderAddress;
            }
        }

        return Error.NotFound("Could not retreive user address");
    }

    public async Task<Resultable> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken)
    {
        using var _ = await cacheAccessMonitor.LockAsync(cancellationToken);
        return await _addressCache.StoreAsync(orderAddress, cancellationToken);
    }
}
