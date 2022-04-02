using System.Net;
using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Services.Addic7ed.EnvVar.Http;

public class HttpProxyParser : IEnvVarParser<HttpProxy>
{
    public HttpProxy Parse(string[] keys, Dictionary<string, string> values)
    {
        var value = values[keys[0]];
        var proxyUri = new Uri(value);
        var userSplit = proxyUri.UserInfo.Split(":");
        return new HttpProxy(new Uri(proxyUri.Scheme + "://" + proxyUri.Authority), new NetworkCredential(userSplit[0], userSplit[1]));
    }
}