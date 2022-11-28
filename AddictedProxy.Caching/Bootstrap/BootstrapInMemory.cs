using AddictedProxy.Caching.OutputCache.Configuration;
using AspNetCoreRateLimit;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Caching.Bootstrap;

public class BootstrapInMemory : IBootstrapConditional, IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // inject counter and rules stores
        services.AddInMemoryRateLimiting();
        
        services.AddDistributedMemoryCache();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("CACHE")?.ToLower() is null or "memory";
    }
}