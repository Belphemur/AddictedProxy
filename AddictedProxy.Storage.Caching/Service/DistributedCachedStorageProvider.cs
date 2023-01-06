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

    public async Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken)
    {
        var stored = await _storageProvider.StoreAsync(filename, inputStream, cancellationToken);
        if (!stored)
        {
            return stored;
        }

        var dataBytes = await GetDataBytesAsync(inputStream, cancellationToken);
        await _distributedCache.SetAsync(filename, dataBytes, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromDays(1)
        }, cancellationToken);

        return stored;
    }

    private static async Task<byte[]> GetDataBytesAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        inputStream.ResetPosition();
        if (inputStream is MemoryStream memStream)
        {
            return memStream.GetBuffer();
        }

        using var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.ResetPosition();
        var dataBytes = memoryStream.GetBuffer();
        return dataBytes;
    }

    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        var cachedData = await _distributedCache.GetAsync(filename, cancellationToken);
        if (cachedData != null)
        {
            return new MemoryStream(cachedData);
        }

        return await _storageProvider.DownloadAsync(filename, cancellationToken);
    }
}