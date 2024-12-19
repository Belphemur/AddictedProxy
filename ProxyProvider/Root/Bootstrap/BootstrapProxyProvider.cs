using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyProvider.Root.Service;
using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Root.Bootstrap;

public class BootstrapProxyProvider : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProxyRotator, ProxyRotator>();
        services.AddHostedService<ProxyUpdaterHostedService>();
    }
}