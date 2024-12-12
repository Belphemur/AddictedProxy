using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyScrape.Model;
using ProxyScrape.Service;

namespace ProxyScrape.Bootstrap;

public class BootstrapProxyScrape : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProxyScrapeConfig>("ProxyScrape", configuration);
        services.AddHttpClient<IProxyScrapeClient, ProxyScrapeClient>(client =>
                client.BaseAddress = new Uri("https://dashboard.proxyscrape.com/v2/v4/"))
            .SetHandlerLifetime(TimeSpan.FromHours(2));
    }
}