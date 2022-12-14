#region

using AddictedProxy.Culture.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Culture.Bootstrap;

public class BootstrapCulture : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICultureParser, CultureParser>();
    }
}