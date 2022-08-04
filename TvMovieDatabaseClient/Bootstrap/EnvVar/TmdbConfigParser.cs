using System.Collections.Generic;
using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace TvMovieDatabaseClient.Bootstrap.EnvVar;

public class TmdbConfigParser : IEnvVarParser<TmdbConfig>
{
    public TmdbConfig Parse(string[] keys, Dictionary<string, string> values)
    {
        return new TmdbConfig(values[keys[0]]);
    }
}