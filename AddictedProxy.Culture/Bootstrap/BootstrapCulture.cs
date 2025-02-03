#region

using AddictedProxy.Culture.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace AddictedProxy.Culture.Bootstrap;

public class BootstrapCulture : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<ICultureParser, CultureParser>();
    }
}