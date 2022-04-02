using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Database.EnvVar;

public class EFCoreLicenseParser : IEnvVarParser<EFCoreLicense>
{
    public EFCoreLicense Parse(string value)
    {
        var split = value.Split(':', 2);
        return new EFCoreLicense(split[0], split[1]);
    }
}