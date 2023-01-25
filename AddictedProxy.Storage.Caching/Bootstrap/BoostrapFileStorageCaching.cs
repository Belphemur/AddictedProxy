using Acrobit.AcroFS;
using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BoostrapFileStorageCaching : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICachedStorageProvider, FileCacheStorageProvider>();
        services.AddSingleton<FileStore>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<StorageCachingConfig>>();
            return FileStore.CreateStore(config.Value.CachePath!, new StoreConfig
            {
                UseSimplePath = true
            });
        });
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var storageConfig = configuration.GetSection("Caching:Storage").Get<StorageCachingConfig>();
        return storageConfig?.StorageType == StorageCachingConfig.Type.File;
    }
}