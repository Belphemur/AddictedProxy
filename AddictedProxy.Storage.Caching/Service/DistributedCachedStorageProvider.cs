using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Extensions;
using AddictedProxy.Storage.Store.Compression;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Performance.Service;
using Prometheus;

namespace AddictedProxy.Storage.Caching.Service;

public class DistributedCachedStorageProvider : ICachedStorageProvider
{
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<StorageCachingConfig> _cachingConfig;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly Counter _cacheHitCounter;
    private readonly Counter _cacheMissCounter;
    private readonly ICompressedStorageProvider _compressedStorageProvider;

    public DistributedCachedStorageProvider(ICompressedStorageProvider compressedCompressedStorageProvider,
                                                      IDistributedCache distributedCache, 
                                                      IOptions<StorageCachingConfig> cachingConfig,
                                                      IPerformanceTracker performanceTracker)
    {
        _compressedStorageProvider = compressedCompressedStorageProvider;
        _distributedCache = distributedCache;
        _cachingConfig = cachingConfig;
        _performanceTracker = performanceTracker;
        _cacheHitCounter = Metrics.CreateCounter("cache_storage_hits", "Number of hits of the storage's cache", new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars(),
            LabelNames = ["sharding_key"]
        });
        _cacheMissCounter = Metrics.CreateCounter("cache_storage_miss", "Number of misses of the storage's cache", new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars(),
            LabelNames = ["sharding_key"]
        });
    }

    private static async Task<MemoryStream> GetMemoryStreamAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        if (inputStream is MemoryStream memStream)
        {
            memStream.ResetPosition();
            return memStream;
        }

        var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.ResetPosition();
        return memoryStream;
    }

    private string GetCacheKey(string sharding, string filename) => $"{{{sharding}}}/{filename}/[v2]";


    public async Task<Stream?> GetSertAsync(string shardingKey, string filename, CancellationToken cancellationToken)
    {
        using var span = _performanceTracker.BeginNestedSpan("getsert-cache-storage", $"Get file {filename}");
        span.SetTag("cache.sharding_key", shardingKey);
        var cacheKey = GetCacheKey(shardingKey, filename);
        var cachedData = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            span.SetTag("cache.result", "hit");
            _cacheHitCounter.WithLabels(shardingKey).Inc();
            return new MemoryStream(cachedData);
        }

        //We use the normal storage provider that will contain the already compressed file
        //because StoreSubtitle use the compressed storage provider
        var stream = await _compressedStorageProvider.DownloadAsync(filename, cancellationToken);
        if (stream == null)
        {
            return null;
        }

        span.SetTag("cache.result", "miss");

        _cacheMissCounter.WithLabels(shardingKey).Inc();

        var memStream = await GetMemoryStreamAsync(stream, cancellationToken);

        await _distributedCache.SetAsync(cacheKey, memStream.GetBuffer(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = _cachingConfig.Value.Sliding,
            AbsoluteExpirationRelativeToNow = _cachingConfig.Value.Absolute
        }, cancellationToken);

        return memStream;
    }
}