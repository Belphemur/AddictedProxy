using AddictedProxy.Storage.Caching.Model;
using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BootstrapStorageCaching : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StorageCachingConfig>(configuration.GetSection("Caching:Storage"));
        services.AddSingleton<ICachedStorageProvider, DistributedCachedStorageProvider>();
    }
}