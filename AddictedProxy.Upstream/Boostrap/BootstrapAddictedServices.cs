#region

using System.Net;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.EnvVar.Http;
using AddictedProxy.Upstream.Service.Performance;
using AngleSharp.Html.Parser;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using InversionOfControl.Service.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Prometheus;

#endregion

namespace AddictedProxy.Upstream.Boostrap;

public class BootstrapAddictedServices : IBootstrap,
    IBootstrapEnvironmentVariable<HttpProxy, HttpProxyParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<IHtmlParser, HtmlParser>();
        services.AddSingleton<Parser>();

        services.AddHttpClient<IAddic7edClient, Addic7edClient>(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.BaseAddress = new Uri("https://www.addic7ed.com");
                })
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>(), false))
                .SetHandlerLifetime(TimeSpan.FromMinutes(3))
                .UseHttpClientMetrics()
                .AddResilienceHandler("addic7ed-client", ConfigureResilience);

        services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(2);
                    client.BaseAddress = new Uri("https://www.addic7ed.com");
                })
                .ConfigurePrimaryHttpMessageHandler(provider => BuildProxyHttpMessageHandler(provider.GetRequiredService<HttpProxy>(), false))
                .SetHandlerLifetime(TimeSpan.FromMinutes(3))
                .UseHttpClientMetrics()
                .AddResilienceHandler("addic7ed-downloader", ConfigureResilience);

        services.AddSingleton<HttpUtils>();
        services.AddSingleton<DownloadCounterWrapper>();
    }

    public EnvVarRegistration<HttpProxy, HttpProxyParser> EnvVarRegistration => new("PROXY_URL");

    private static HttpMessageHandler BuildProxyHttpMessageHandler(HttpProxy proxy, bool followRedirect = true)
    {
        return new SocketsHttpHandler
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

    private static void ConfigureResilience(ResiliencePipelineBuilder<HttpResponseMessage> builder)
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            ShouldHandle = args => ValueTask.FromResult(
                args.Outcome.Result is HttpResponseMessage response &&
                IsRetryableStatusCode(response.StatusCode)),
            BackoffType = DelayBackoffType.Exponential,
            MaxRetryAttempts = 8,
            Delay = TimeSpan.FromSeconds(10),
            MaxDelay = TimeSpan.FromSeconds(60)
        });

        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            ShouldHandle = args => ValueTask.FromResult(
                args.Outcome.Result is HttpResponseMessage response &&
                IsCircuitBreakerStatusCode(response.StatusCode)),
            SamplingDuration = TimeSpan.FromMinutes(1),
            FailureRatio = 0.5,
            MinimumThroughput = 20,
            BreakDuration = TimeSpan.FromMinutes(5)
        });

        builder.AddTimeout(new HttpTimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(60)
        });
    }

    private static bool IsRetryableStatusCode(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;
        // Retry transient server errors, auth failures and the legacy Addic7ed 404/403 cases.
        return status is >= 500 and <= 599 or 401 or 402 or 403 or 404;
    }

    private static bool IsCircuitBreakerStatusCode(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;
        // Open the circuit when upstream is failing with server errors or explicit auth errors.
        return status is >= 500 and <= 599 or 401 or 402 or 403;
    }
}