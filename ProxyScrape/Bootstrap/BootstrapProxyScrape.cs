using InversionOfControl.Model;
using InversionOfControl.Service.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProxyScrape.Model;
using ProxyScrape.Service;

namespace ProxyScrape.Bootstrap;

public class BootstrapProxyScrape : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<ProxyScrapeConfig>(configuration.GetSection("ProxyScrape"));
        services.AddHttpClient<IProxyScrapeClient, ProxyScrapeClient>(client =>
                client.BaseAddress = new Uri("https://api.proxyscrape.com/"))
            .SetHandlerLifetime(TimeSpan.FromMinutes(10))
            .AddSharedResilienceHandler("proxyscrape");

        if (configuration.GetValue("ProxyScrape:EnableScrape", false))
        {
            services.AddHostedService<MetricGatherHostedService>();
        }
    }
}
