using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProxyProvider.GeoList.Service;

namespace ProxyProvider.GeoList.Bootstrap;

public class BootstrapGeoList : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddHttpClient<IGeoListProxyFetcher, GeoListProxyFetcher>()
            .SetHandlerLifetime(TimeSpan.FromHours(1))
            .AddStandardResilienceHandler();
    }
}