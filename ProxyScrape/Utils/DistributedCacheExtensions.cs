using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace ProxyScrape.Utils;

internal static class DistributedCacheExtensions
{
    public record struct CacheData<T>(T Value, DistributedCacheEntryOptions Options) where T : class?;

    /// <summary>
    /// Get a value from the cache and deserialize it
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);
        if (bytes is null)
        {
            return default;
        }

        using var memoryStream = new MemoryStream(bytes);
        return await JsonSerializer.DeserializeAsync<T>(memoryStream, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Get or upsert data in the cache
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="fallback"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> GetSertAsync<T>(this IDistributedCache cache, string key, Func<Task<CacheData<T?>?>> fallback) where T : class?
    {
        var result = await cache.GetAsync<T>(key);
        if (result is null)
        {
            var data = await fallback();
            if (data?.Value == null)
            {
                return null;
            }

            result = data.Value.Value;
            await cache.SetAsync(key, data.Value.Value, data.Value.Options);
        }

        return result;
    }

    /// <summary>
    /// Get or upsert data in the cache
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="fallback"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> GetSertAsync<T>(this IDistributedCache cache, string key, Func<Task<CacheData<T>>> fallback) where T : class?
    {
        var result = await cache.GetAsync<T>(key);
        if (result is null)
        {
            var data = await fallback();
            result = data.Value;
            await cache.SetAsync(key, data.Value, data.Options);
        }

        return result;
    }

    /// <summary>
    /// Set a value in the cache and serialize it
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        await cache.SetAsync(key, bytes, options, cancellationToken);
    }
}