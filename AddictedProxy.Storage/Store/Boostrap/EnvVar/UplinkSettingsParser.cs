using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Storage.Store.Boostrap.EnvVar;

public class UplinkSettingsParser : IEnvVarParser<UplinkSettings>
{
    public UplinkSettings Parse(string[] keys, Dictionary<string, string> values)
        => new UplinkSettings(values[keys[0]], values[keys[1]]);
}