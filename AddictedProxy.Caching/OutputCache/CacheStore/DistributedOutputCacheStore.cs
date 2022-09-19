using System.Text.Json;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;

namespace AddictedProxy.Caching.OutputCache.CacheStore;

public class DistributedOutputCacheStore : IOutputCacheStore
{
    private readonly IDistributedCache _distributedCache;

    public DistributedOutputCacheStore(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    private static string TagKeyGeneration(string tag) => $"tag/{tag}/keys";

    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        var tagKey = TagKeyGeneration(tag);
        var serializedKeys = await _distributedCache.GetStringAsync(tagKey, cancellationToken) ?? "[]";
        var keys = JsonSerializer.Deserialize<HashSet<string>>(serializedKeys)!;
        foreach (var key in keys)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        await _distributedCache.RemoveAsync(tagKey, cancellationToken);
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetAsync(key, cancellationToken);
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        await _distributedCache.SetAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = validFor
        }, cancellationToken);
        
        foreach (var tag in tags ?? Array.Empty<string>())
        {
            var tagKey = TagKeyGeneration(tag);
            var serializedKeys = await _distributedCache.GetStringAsync(tagKey, cancellationToken) ?? "[]";
            var keys = JsonSerializer.Deserialize<HashSet<string>>(serializedKeys)!;
            keys.Add(key);

            await _distributedCache.SetAsync(tagKey, JsonSerializer.SerializeToUtf8Bytes(keys), cancellationToken);
        }
    }
}