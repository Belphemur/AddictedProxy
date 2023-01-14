using AddictedProxy.Caching.Bootstrap.EnvVar;
using StackExchange.Redis;

namespace AddictedProxy.Caching.Redis;

public class RedisConnection
{
    public static readonly RedisConnection Instance = new();

    private IConnectionMultiplexer? _connection;

    /// <summary>
    /// Get or create a connection to Redis
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public IConnectionMultiplexer GetOrCreateConnection(RedisConfig config)
    {
        if (_connection != null)
        {
            return _connection;
        }

        var configurationOptions = GetConfigurationOptions(config);

        return _connection = ConnectionMultiplexer.Connect(configurationOptions);
    }

    private static ConfigurationOptions GetConfigurationOptions(RedisConfig config)
    {
        var configurationOptions = ConfigurationOptions.Parse(config.Connection);
        configurationOptions.SyncTimeout = (int)config.Timeout.TotalMilliseconds;
        return configurationOptions;
    }

    /// <summary>
    /// Get or create a connection to Redis
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<IConnectionMultiplexer> GetOrCreateConnectionAsync(RedisConfig config)
    {
        if (_connection != null)
        {
            return _connection;
        }

        var configurationOptions = GetConfigurationOptions(config);

        return _connection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
    }
}