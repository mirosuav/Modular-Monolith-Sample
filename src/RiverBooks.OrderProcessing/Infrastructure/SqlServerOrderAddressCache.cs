using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Application.Interfaces;
using RiverBooks.OrderProcessing.Domain;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal class SqlServerOrderAddressCache(
    ILogger<SqlServerOrderAddressCache> logger,
    IDistributedCache cache) : IOrderAddressCache
{
    public async Task<ResultOf<OrderAddress>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var address = await cache.GetValueAsync<OrderAddress>(id.ToString(), cancellationToken);

        if (address is null)
        {
            logger.LogWarning("Address {id} not found in {db}", id, "Distributed cache");
            return Error.NotFound("User address not found");
        }

        logger.LogInformation("Address {id} returned from {db}", id, "Distributed cache");
        return address;
    }

    public async Task<ResultOf> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken)
    {
        var key = orderAddress.Id.ToString();
        await cache.SetAsync(key, orderAddress, cancellationToken);
        logger.LogInformation("Address {id} stored in {db}", key, "Distributed cache");
        return true;
    }
}