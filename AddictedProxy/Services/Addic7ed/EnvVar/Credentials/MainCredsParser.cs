using InversionOfControl.Service.EnvironmentVariable.Parser;
using InversionOfControl.Service.EnvironmentVariable.Registration;

namespace AddictedProxy.Model.Config;

public class MainCredsParser : IEnvVarParser<MainCreds>
{
    public MainCreds Parse(string[] keys, Dictionary<string, string> values)
    {
        var userName = values[keys[0]];
        var password = values[keys[1]];

        return new MainCreds(int.Parse(userName), password);
    }
}