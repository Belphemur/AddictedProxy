using AddictedProxy.Caching.Bootstrap.EnvVar;
using AspNetCoreRateLimit;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Caching.Bootstrap;

public class BootstrapDistributedCaching : IBootstrap, IBootstrapConditional, IBootstrapEnvironmentVariable<RedisConfig, RedisConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "Addicted";
            
        });
        services.AddDistributedRateLimiting();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("CACHE")?.ToLower() == "redis";
    }

    public EnvVarRegistration<RedisConfig, RedisConfigParser> EnvVarRegistration { get; } = new("REDIS_CONNECTION");
}