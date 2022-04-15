#region

using AddictedProxy.Services.Provider.Subtitle;
using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Services.Saver.Bootstrap;

public class BootstrapSaver : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IShowProvider, ShowProvider>();
        services.AddScoped<ISubtitleProvider, SubtitleProvider>();
    }
}