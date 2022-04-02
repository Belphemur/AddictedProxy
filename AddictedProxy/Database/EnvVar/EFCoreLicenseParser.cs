using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Database.EnvVar;

public class EFCoreLicenseParser : IEnvVarParser<EFCoreLicense>
{
    public EFCoreLicense Parse(string[] keys, Dictionary<string, string> values)
    {
        var value = values[keys[0]];
        var split = value.Split(':', 2);
        return new EFCoreLicense(split[0], split[1]);
    }
}