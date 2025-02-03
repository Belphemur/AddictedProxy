using AddictedProxy.Storage.Caching.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BootstrapDistributedStorageCaching : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<ICachedStorageProvider, DistributedCachedStorageProvider>();
    }
}