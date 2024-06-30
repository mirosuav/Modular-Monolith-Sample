using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Extensions;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal class SqlServerOrderAddressCache(
    ILogger<SqlServerOrderAddressCache> logger,
    IDistributedCache cache) : IOrderAddressCache
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<SqlServerOrderAddressCache> _logger = logger;

    public async Task<Resultable<OrderAddress>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var address = await _cache.GetValueAsync<OrderAddress>(id.ToString(), cancellationToken);

        if (address is null)
        {
            _logger.LogWarning("Address {id} not found in {db}", id, "Distributed cache");
            return Error.NotFound("User address not found");
        }

        _logger.LogInformation("Address {id} returned from {db}", id, "Distributed cache");
        return Resultable.Success(address);
    }

    public async Task<Resultable> StoreAsync(OrderAddress orderAddress, CancellationToken cancellationToken)
    {
        var key = orderAddress.Id.ToString();
        await _cache.SetAsync(key, orderAddress, cancellationToken);
        _logger.LogInformation("Address {id} stored in {db}", key, "Distributed cache");
        return Resultable.Success();
    }
}
