using System.Text.Json;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;

namespace AddictedProxy.Caching.OutputCache.CacheStore;

public class DistributedOutputCacheStore : IOutputCacheStore
{
    private readonly IDistributedCache _distributedCache;

    private static string Key(string key) => $"Cache.{key}";

    public DistributedOutputCacheStore(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetAsync(Key(key), cancellationToken);
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        await _distributedCache.SetAsync(Key(key), value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = validFor
        }, cancellationToken);
    }
}