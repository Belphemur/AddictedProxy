using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProxyProvider.Root.Service;
using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Root.Bootstrap;

public class BootstrapProxyProvider : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<IProxyRotator, ProxyRotator>();
        services.AddHostedService<ProxyUpdaterHostedService>();
    }
}