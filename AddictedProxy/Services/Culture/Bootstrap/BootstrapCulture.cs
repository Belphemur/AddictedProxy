#region

using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Services.Culture.Bootstrap;

public class BootstrapCulture : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<CultureParser>();
    }
}