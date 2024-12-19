#region

using System.Net;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.EnvVar.Http;
using AddictedProxy.Upstream.Service.Performance;
using AngleSharp.Html.Parser;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;
using Prometheus;

#endregion

namespace AddictedProxy.Upstream.Boostrap;

public class BootstrapAddictedServices : IBootstrap,
    IBootstrapEnvironmentVariable<HttpProxy, HttpProxyParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHtmlParser, HtmlParser>();
        services.AddSingleton<Parser>();

        services.AddHttpClient<IAddic7edClient, Addic7edClient>(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(30);
                    client.BaseAddress = new Uri("https://www.addic7ed.com");
                })
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>(), false))
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .UseHttpClientMetrics();

        services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(2);
                    client.BaseAddress = new Uri("https://www.addic7ed.com");
                })
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>(), false))
                .SetHandlerLifetime(TimeSpan.FromMinutes(1))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .UseHttpClientMetrics();

        services.AddSingleton<HttpUtils>();
        services.AddSingleton<DownloadCounterWrapper>();
    }

    public EnvVarRegistration<HttpProxy, HttpProxyParser> EnvVarRegistration => new("PROXY_URL");

    private static HttpMessageHandler BuildProxyHttpMessageHandler(HttpProxy proxy, bool followRedirect = true)
    {
        return new HttpClientHandler
        {
            AllowAutoRedirect = followRedirect,
            Proxy = new WebProxy
            {
                Address = proxy.Address,
                Credentials = proxy.Credentials
            },
            AutomaticDecompression = DecompressionMethods.All
        };
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreaker() => HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromMinutes(1), 20, TimeSpan.FromMinutes(5));

    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy() => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(60));

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(10), retryCount: 8);
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            //Issue with downloading the subtitle from Addic7ed
            .OrInner<IOException>()
            .Or<TimeoutRejectedException>()
            .OrResult(msg => msg.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(delay);
    }
}