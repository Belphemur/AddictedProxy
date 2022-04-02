using InversionOfControl.Model;

namespace AddictedProxy.Services.Saver.Bootstrap;

public class BootstrapSaver : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IAddictedSaver, AddictedSaver>();
    }
}