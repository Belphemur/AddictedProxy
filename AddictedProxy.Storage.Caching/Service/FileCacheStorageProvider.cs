using Acrobit.AcroFS;
using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Compressor;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Storage.Caching.Service;

public class FileCacheStorageProvider : ICachedStorageProvider
{
    private readonly IOptions<StorageCachingConfig> _cachingConfig;
    private readonly FileStore _fileStore;

    public FileCacheStorageProvider(IOptions<StorageCachingConfig> cachingConfig, FileStore fileStore)
    {
        _cachingConfig = cachingConfig;
        _fileStore = fileStore;
    }
    public Task<Stream?> GetSertAsync(string shardingKey, string filename, CancellationToken cancellationToken)
    {
        //var stream =  _fileStore.StoreStream()
        return null;
    }
}