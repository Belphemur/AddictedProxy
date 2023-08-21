using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BootstrapDistributedStorageCaching : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICachedStorageProvider, DistributedCompressedCachedStorageProvider>();
    }
}