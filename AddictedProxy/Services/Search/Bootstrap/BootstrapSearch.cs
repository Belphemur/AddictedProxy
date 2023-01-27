using AddictedProxy.Services.Search.Subtitle;
using InversionOfControl.Model;

namespace AddictedProxy.Services.Search.Bootstrap;

public class BootstrapSearch : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISearchSubtitlesService, SearchSubtitlesService>();
    }
}