using AddictedProxy.Storage.Caching.Model;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Storage.Caching.Bootstrap;

public class BootstrapStorageCaching : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<StorageCachingConfig>(configuration.GetSection("Caching:Storage"));
    }
}