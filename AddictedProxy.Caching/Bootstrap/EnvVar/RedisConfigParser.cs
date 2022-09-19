using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Caching.Bootstrap.EnvVar;

public class RedisConfigParser : IEnvVarParser<RedisConfig>
{
    public RedisConfig Parse(string[] keys, Dictionary<string, string> values)
    {
        return new RedisConfig(values[keys[0]]);
    }
}