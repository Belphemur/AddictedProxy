using System.Net;
using InversionOfControl.Service.EnvironmentVariable.Parser;

namespace AddictedProxy.Services.Addic7ed.EnvVar;

public class HttpProxyParser : IEnvVarParser<HttpProxy>
{
    public HttpProxy Parse(string value)
    {
        var proxyUri = new Uri(value.Trim('"'));
        var userSplit = proxyUri.UserInfo.Split(":");
        return new HttpProxy(new Uri(proxyUri.Scheme + "://" + proxyUri.Authority), new NetworkCredential(userSplit[0], userSplit[1]));

    }
}