using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Compressor;
using AddictedProxy.Storage.Store;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NeoSmart.Caching.Sqlite;
using Performance.Service;

namespace AddictedProxy.Storage.Caching.Service;

public class SqliteCacheStorageProvider : DistributedCachedStorageProvider
{
    public SqliteCacheStorageProvider(IStorageProvider storageProvider, IDistributedCache distributedCache, IOptions<StorageCachingConfig> cachingConfig, ICompressor compressor, IPerformanceTracker performanceTracker) : base(storageProvider, distributedCache, cachingConfig, compressor, performanceTracker)
    {
    }
}

