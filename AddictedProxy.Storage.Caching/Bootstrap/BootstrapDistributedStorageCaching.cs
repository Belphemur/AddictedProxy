using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BootstrapDistributedStorageCaching : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICachedStorageProvider, DistributedCachedStorageProvider>();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var storageConfig = configuration.GetSection("Caching:Storage").Get<StorageCachingConfig>();
        return storageConfig?.StorageType == StorageCachingConfig.Type.Distributed;
    }
}