using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeoSmart.Caching.Sqlite;
using Prometheus;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BoostrapSqliteStorageCaching : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var storageConfig = configuration.GetSection("Caching:Storage").Get<StorageCachingConfig>();
        services.AddSingleton<ICachedStorageProvider, SqliteCacheStorageProvider>();
        services.AddSqliteCache(options =>
        {
            options.VacuumOption = SqliteCacheOptions.Vacuum.Full;
            var storageConfigCachePath = string.IsNullOrEmpty(storageConfig?.CachePath) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Addicted") : storageConfig.CachePath;
            options.CachePath = Path.Combine(storageConfigCachePath, "sqlite-cache.db");
            options.Prometheus = new SqliteCacheOptions.Metrics(Metrics.CreateCounter("cache_storage_evictions", "nb of cache eviction"));
        });
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var storageConfig = configuration.GetSection("Caching:Storage").Get<StorageCachingConfig>();
        return storageConfig?.StorageType == StorageCachingConfig.Type.Sqlite;
    }
}