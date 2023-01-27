using AddictedProxy.Caching.Bootstrap.EnvVar;
using AddictedProxy.Caching.Redis;
using InversionOfControl.Model;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AddictedProxy.Caching.Bootstrap;

public class BootstrapDistributedCaching : IBootstrap, IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("Redis").Get<RedisConfig>()!;

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () => RedisConnection.Instance.GetOrCreateConnectionAsync(config);
            options.InstanceName = config.InstanceName;
        });
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("CACHE")?.ToLower() == "redis";
    }
}