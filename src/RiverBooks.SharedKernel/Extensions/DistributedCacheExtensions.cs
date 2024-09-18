using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace RiverBooks.SharedKernel.Extensions;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions CacheJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        CancellationToken cancellationToken)
    {
        return SetAsync(cache, key, value, new DistributedCacheEntryOptions(), cancellationToken);
    }

    public static Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CacheJsonSerializerOptions));
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public static async Task<T?> GetValueAsync<T>(
        this IDistributedCache cache,
        string key,
        CancellationToken cancellationToken)
    {
        var cacheBytes = await cache.GetAsync(key, cancellationToken);

        if (cacheBytes is null or [])
            return default;

        return JsonSerializer.Deserialize<T>(cacheBytes, CacheJsonSerializerOptions);
    }
}