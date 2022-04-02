using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Services.Addic7ed.EnvVar.Credentials;

public class MainCredsParser : IEnvVarParser<MainCreds>
{
    public MainCreds Parse(string[] keys, Dictionary<string, string> values)
    {
        var userName = values[keys[0]];
        var password = values[keys[1]];

        return new MainCreds(int.Parse(userName), password);
    }
}