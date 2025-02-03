using System.Net;
using System.Reflection;
using AntiCaptcha.Bootstrap;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProxyScrape.Model;
using ProxyScrape.Service;

namespace ProxyScrape.Bootstrap;

public class BootstrapProxyScrape : IBootstrap
{
    public Assembly[] Dependencies => [typeof(BootstrapAntiCaptcha).Assembly];

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<ProxyScrapeConfig>(configuration.GetSection("ProxyScrape"));
        services.AddHttpClient<IProxyScrapeClient, ProxyScrapeClient>(client =>
                client.BaseAddress = new Uri("https://dashboard.proxyscrape.com/"))
            .SetHandlerLifetime(TimeSpan.FromMinutes(10))
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            }).AddStandardResilienceHandler();

        // Only load the MetricGatherHostedService in release mode to avoid using anti-captcha quota
#if !DEBUG
        services.AddHostedService<MetricGatherHostedService>();
#endif
    }
}