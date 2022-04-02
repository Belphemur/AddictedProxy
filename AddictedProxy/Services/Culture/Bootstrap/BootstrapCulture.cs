using InversionOfControl.Model;

namespace AddictedProxy.Services.Culture.Bootstrap;

public class BootstrapCulture : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CultureParser>();
    }
}