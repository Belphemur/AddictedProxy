#region

using System.Net;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.EnvVar.Http;
using AngleSharp.Html.Parser;
using Bogus;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;
using Sentry;

#endregion

namespace AddictedProxy.Upstream.Boostrap;

public class BootstrapAddictedServices : IBootstrap,
                                         IBootstrapEnvironmentVariable<HttpProxy, HttpProxyParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHtmlParser, HtmlParser>();
        services.AddSingleton<Parser>();

        services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                .ConfigurePrimaryHttpMessageHandler(provider => new SentryHttpMessageHandler(BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>())))
                .SetHandlerLifetime(TimeSpan.FromMinutes(15))
                .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>()
                .ConfigurePrimaryHttpMessageHandler(provider => new SentryHttpMessageHandler(BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>())))
                .SetHandlerLifetime(TimeSpan.FromMinutes(15))
                .AddPolicyHandler(GetRetryPolicy());

        services.AddSingleton<Faker>();
        services.AddSingleton<HttpUtils>();
    }

    public EnvVarRegistration<HttpProxy, HttpProxyParser> EnvVarRegistration => new("PROXY_URL");

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
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 10);
        return HttpPolicyExtensions
               .HandleTransientHttpError()
               .Or<TimeoutRejectedException>()
               //Issue with downloading the subtitle from Addic7ed
               .Or<HttpRequestException>(exception => exception.InnerException is IOException)
               .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.Forbidden)
               .WaitAndRetryAsync(delay)
               .WrapAsync(Policy.TimeoutAsync(30));
    }
}