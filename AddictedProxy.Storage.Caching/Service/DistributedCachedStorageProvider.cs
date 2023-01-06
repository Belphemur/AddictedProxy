using AddictedProxy.Storage.Extensions;
using AddictedProxy.Storage.Store;
using Microsoft.Extensions.Caching.Distributed;

namespace AddictedProxy.Storage.Caching.Service;

public class DistributedCachedStorageProvider : ICachedStorageProvider
{
    private readonly IStorageProvider _storageProvider;
    private readonly IDistributedCache _distributedCache;

    public DistributedCachedStorageProvider(IStorageProvider storageProvider, IDistributedCache distributedCache)
    {
        _storageProvider = storageProvider;
        _distributedCache = distributedCache;
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
            SlidingExpiration = TimeSpan.FromDays(1)
        }, cancellationToken);

        return memStream;
    }
}