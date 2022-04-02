using System.Net;
using AddictedProxy.Model.Config;
using AngleSharp.Html.Parser;
using InversionOfControl.Model;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;

namespace AddictedProxy.Services.Addic7ed.Boostrap;

public class BootstrapAddictedServices : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
              
        services.AddSingleton<MainCreds>();
        services.AddSingleton<IHtmlParser, HtmlParser>();
        services.AddSingleton<Parser>();

        services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                .ConfigurePrimaryHttpMessageHandler(_ => BuildProxyHttpMessageHandler())
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(Policy.RateLimitAsync<HttpResponseMessage>(
                    3, TimeSpan.FromSeconds(1), 10
                ));

        services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>()
                .ConfigurePrimaryHttpMessageHandler(_ => BuildProxyHttpMessageHandler())
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy());
    }

    private static HttpMessageHandler BuildProxyHttpMessageHandler()
    {
        var proxyUrl = Environment.GetEnvironmentVariable("PROXY_URL") ?? throw new InvalidOperationException("No proxy set");
        var proxyUri = new Uri(proxyUrl.Trim('"'));
        var userSplit = proxyUri.UserInfo.Split(":");
        return new HttpClientHandler
        {
            Proxy = new WebProxy
            {
                Address = new Uri(proxyUri.Scheme + "://" + proxyUri.Authority),
                Credentials = new NetworkCredential(userSplit[0], userSplit[1])
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