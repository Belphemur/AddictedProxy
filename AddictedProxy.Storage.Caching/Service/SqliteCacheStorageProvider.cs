using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Compressor;
using AddictedProxy.Storage.Store;
using Microsoft.Extensions.Options;
using NeoSmart.Caching.Sqlite;

namespace AddictedProxy.Storage.Caching.Service;

public class SqliteCacheStorageProvider : DistributedCachedStorageProvider
{
    public SqliteCacheStorageProvider(IStorageProvider storageProvider, SqliteCache distributedCache, IOptions<StorageCachingConfig> cachingConfig, ICompressor compressor) : base(storageProvider, distributedCache, cachingConfig, compressor)
    {
    }
}

