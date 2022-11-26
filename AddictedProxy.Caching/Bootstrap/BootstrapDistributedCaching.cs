using AddictedProxy.Caching.Bootstrap.EnvVar;
using AddictedProxy.Caching.OutputCache;
using AddictedProxy.Caching.OutputCache.CacheStore;
using AddictedProxy.Caching.OutputCache.Configuration;
using AspNetCoreRateLimit;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Caching.Bootstrap;

public class BootstrapDistributedCaching : IBootstrap, IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("Redis").Get<RedisConfig>()!;

        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = config.InstanceName;
            options.Configuration = config.Connection;
        });
        services.AddDistributedRateLimiting();

        //Disable output caching in Redis, more problem than solution
        //services.TryAddSingleton<IOutputCacheStore, DistributedOutputCacheStore>();
        services.AddOutputCache(options =>
        {
            options.AddOwnPolicies();
        });
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("CACHE")?.ToLower() == "redis";
    }
}