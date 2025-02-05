#region

using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.ShowInfo;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Hub;
using AddictedProxy.Services.Provider.Subtitle;
using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Services.Provider.Bootstrap;

public class BootstrapProvider : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IShowRefresher, ShowRefresher>();
        services.AddScoped<ISubtitleProvider, SubtitleProvider>();
        services.AddScoped<ISeasonRefresher, SeasonRefresher>();
        services.AddScoped<IEpisodeRefresher, EpisodeRefresher>();
        services.Configure<RefreshConfig>(configuration.GetSection("Refresh"));
        services.AddSingleton<IRefreshHubManager, RefreshHubManager>();
        services.AddScoped<SubtitleCounterUpdater>();
        services.AddScoped<IDetailsProvider, DetailsProvider>();
    }
}