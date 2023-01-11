using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Extensions;
using AddictedProxy.Storage.Store;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Storage.Caching.Service;

public class DistributedCachedStorageProvider : ICachedStorageProvider
{
    private readonly IStorageProvider _storageProvider;
    private readonly IDistributedCache _distributedCache;
    private readonly IOptions<StorageCachingConfig> _cachingConfig;

    public DistributedCachedStorageProvider(IStorageProvider storageProvider, IDistributedCache distributedCache, IOptions<StorageCachingConfig> cachingConfig)
    {
        _storageProvider = storageProvider;
        _distributedCache = distributedCache;
        _cachingConfig = cachingConfig;
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

    private string GetCacheKey(string sharding, string filename) => $"{{{sharding}}}/{filename}";


    public async Task<Stream?> GetSertAsync(string shardingKey, string filename, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(shardingKey, filename);
        var cachedData = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            return new MemoryStream(cachedData);
        }

        var stream = await _storageProvider.DownloadAsync(filename, cancellationToken);
        if (stream == null)
        {
            return stream;
        }

        var memStream = await GetMemoryStreamAsync(stream, cancellationToken);

        await _distributedCache.SetAsync(cacheKey, memStream.GetBuffer(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = _cachingConfig.Value.Sliding,
            AbsoluteExpirationRelativeToNow = _cachingConfig.Value.Absolute
        }, cancellationToken);

        return memStream;
    }
}