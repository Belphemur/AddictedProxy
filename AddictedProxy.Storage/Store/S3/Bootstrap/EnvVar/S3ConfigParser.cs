using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;

public class S3ConfigParser : IEnvVarParser<S3Config>
{
    public S3Config Parse(string[] keys, Dictionary<string, string> values)
    {
        return new S3Config(values[keys[0]], values[keys[1]], values[keys[2]], values[keys[3]]);
    }
}