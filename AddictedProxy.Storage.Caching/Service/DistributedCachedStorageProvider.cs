using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Extensions;
using AddictedProxy.Storage.Store;
using Compressor;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Performance.Service;
using Prometheus;

namespace AddictedProxy.Storage.Caching.Service;

public class DistributedCachedStorageProvider : ICachedStorageProvider
{
    private readonly IStorageProvider _storageProvider;
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<StorageCachingConfig> _cachingConfig;
    private readonly ICompressor _compressor;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly Counter _cacheHitCounter;
    private readonly Counter _cacheMissCounter;

    public DistributedCachedStorageProvider(IStorageProvider storageProvider, IDistributedCache distributedCache, IOptions<StorageCachingConfig> cachingConfig, ICompressor compressor, IPerformanceTracker performanceTracker)
    {
        _storageProvider = storageProvider;
        _distributedCache = distributedCache;
        _cachingConfig = cachingConfig;
        _compressor = compressor;
        _performanceTracker = performanceTracker;
        _cacheHitCounter = Metrics.CreateCounter("cache_storage_hits", "Number of hits of the storage's cache", new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars()
        });
        _cacheMissCounter = Metrics.CreateCounter("cache_storage_miss", "Number of misses of the storage's cache", new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars()
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

    private string GetCacheKey(string sharding, string filename) => _compressor.GetFileName($"{{{sharding}}}/{filename}");


    public async Task<Stream?> GetSertAsync(string shardingKey, string filename, CancellationToken cancellationToken)
    {
        using var span = _performanceTracker.BeginNestedSpan("getsert-cache-storage", $"Get file {filename}");
        var cacheKey = GetCacheKey(shardingKey, filename);
        var cachedData = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            span.SetTag("cache.result", "hit");
            _cacheHitCounter.Inc();
            var memoryStream = new MemoryStream(cachedData);
            return await _compressor.DecompressAsync(memoryStream, cancellationToken);
        }

        //We use the normal storage provider that will contain the already compressed file
        //because StoreSubtitle use the compressed storage provider
        var stream = await _storageProvider.DownloadAsync(_compressor.GetFileName(filename), cancellationToken);
        if (stream == null)
        {
            return stream;
        }

        span.SetTag("cache.result", "miss");

        _cacheMissCounter.Inc();

        var memStream = await GetMemoryStreamAsync(stream, cancellationToken);

        await _distributedCache.SetAsync(cacheKey, memStream.GetBuffer(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = _cachingConfig.Value.Sliding,
            AbsoluteExpirationRelativeToNow = _cachingConfig.Value.Absolute
        }, cancellationToken);

        return await _compressor.DecompressAsync(memStream, cancellationToken);
    }
}