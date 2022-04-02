using System.Net;
using AddictedProxy.Services.Addic7ed.EnvVar.Credentials;
using AddictedProxy.Services.Addic7ed.EnvVar.Http;
using AngleSharp.Html.Parser;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;

namespace AddictedProxy.Services.Addic7ed.Boostrap;

public class BootstrapAddictedServices : IBootstrap,
                                         IBootstrapEnvironmentVariable<HttpProxy, HttpProxyParser>,
                                         IBootstrapEnvironmentVariable<MainCreds, MainCredsParser>
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHtmlParser, HtmlParser>();
        services.AddSingleton<Parser>();

        services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>()))
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>()
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>()))
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy());
    }

    public EnvVarRegistration<HttpProxy, HttpProxyParser> EnvVarRegistration => new("PROXY_URL");
    EnvVarRegistration<MainCreds, MainCredsParser> IBootstrapEnvironmentVariable<MainCreds, MainCredsParser>.EnvVarRegistration => new("ADDICTED_USERID", "ADDICTED_PASS");

    private static HttpMessageHandler BuildProxyHttpMessageHandler(HttpProxy proxy)
    {
        return new HttpClientHandler
        {
            Proxy = new WebProxy
            {
                Address = proxy.Address,
                Credentials = proxy.Credentials
            }
        };
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var jitterer = new Random();
        return HttpPolicyExtensions
               .HandleTransientHttpError()
               .Or<TimeoutRejectedException>()
               .Or<RateLimitRejectedException>()
               .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.Forbidden)
               .WaitAndRetryAsync(8, // exponential back-off plus some jitter
                   retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                   + TimeSpan.FromMilliseconds(jitterer.Next(0, 300))
               ).WrapAsync(Policy.TimeoutAsync(10));
    }
}