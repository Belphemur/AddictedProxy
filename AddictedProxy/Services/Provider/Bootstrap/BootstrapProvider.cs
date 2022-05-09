#region

using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;

#endregion

namespace AddictedProxy.Services.Provider.Bootstrap;

public class BootstrapProvider : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddJob<StoreSubtitleJob>();
        services.AddJob<FetchSubtitlesJob>();
        services.AddJob<RefreshShowJob>();
        services.AddScoped<IShowRefresher, ShowRefresher>();
        services.AddScoped<ISubtitleProvider, SubtitleProvider>();
        services.AddScoped<ISeasonRefresher, SeasonRefresher>();
        services.AddScoped<IEpisodeRefresher, EpisodeRefresher>();
        services.Configure<RefreshConfig>(configuration.GetSection("Refresh"));

    }
}