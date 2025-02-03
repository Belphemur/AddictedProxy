using AddictedProxy.Services.Search.Subtitle;
using InversionOfControl.Model;

namespace AddictedProxy.Services.Search.Bootstrap;

public class BootstrapSearch : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<ISearchSubtitlesService, SearchSubtitlesService>();
    }
}