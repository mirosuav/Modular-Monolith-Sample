using Microsoft.Extensions.Logging;
using RiverBooks.OrderProcessing.Interfaces;
using RiverBooks.SharedKernel.Helpers;
using StackExchange.Redis;
using System.Text.Json;

namespace RiverBooks.OrderProcessing.Infrastructure;

internal class RedisOrderAddressCache : IOrderAddressCache
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisOrderAddressCache> _logger;

    public RedisOrderAddressCache(ILogger<RedisOrderAddressCache> logger)
    {
        var redis = ConnectionMultiplexer.Connect("localhost"); // TODO: Get server from config
        _db = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<Resultable<OrderAddress>> GetByIdAsync(Guid id)
    {
        string? fetchedJson = await _db.StringGetAsync(id.ToString());

        if (fetchedJson is null)
        {
            _logger.LogWarning("Address {id} not found in {db}", id, "REDIS");
            return Error.CreateNotFound("User address not found");
        }
        var address = JsonSerializer.Deserialize<OrderAddress>(fetchedJson);

        if (address is null)
            return Error.CreateNotFound("User address not found");

        _logger.LogInformation("Address {id} returned from {db}", id, "REDIS");
        return Resultable.Success(address);
    }

    public async Task<Resultable> StoreAsync(OrderAddress orderAddress)
    {
        var key = orderAddress.Id.ToString();
        var addressJson = JsonSerializer.Serialize(orderAddress);

        await _db.StringSetAsync(key, addressJson);
        _logger.LogInformation("Address {id} stored in {db}", orderAddress.Id, "REDIS");

        return Resultable.Success();
    }
}
