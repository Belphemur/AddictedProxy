#region

using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.ShowInfo;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Hub;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Download;
using AddictedProxy.Services.Provider.SuperSubtitles.Config;
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

        // Subtitle downloaders (one per provider)
        services.AddScoped<ISubtitleDownloader, Addic7edSubtitleDownloader>();
        services.AddScoped<ISubtitleDownloader, SuperSubtitlesSubtitleDownloader>();
        services.AddScoped<SubtitleDownloaderFactory>();

        // Provider-specific show refreshers
        services.AddScoped<IProviderShowRefresher, Addic7edShowRefresher>();
        services.AddScoped<IProviderShowRefresher, SuperSubtitlesShowRefresher>();
        services.AddScoped<ProviderShowRefresherFactory>();

        // Provider-specific season refreshers
        services.AddScoped<IProviderSeasonRefresher, Addic7edSeasonRefresher>();
        services.AddScoped<IProviderSeasonRefresher, SuperSubtitlesSeasonRefresher>();
        services.AddScoped<ProviderSeasonRefresherFactory>();

        // Provider-specific episode refreshers
        services.AddScoped<IProviderEpisodeRefresher, Addic7edEpisodeRefresher>();
        services.AddScoped<IProviderEpisodeRefresher, SuperSubtitlesEpisodeRefresher>();
        services.AddScoped<ProviderEpisodeRefresherFactory>();

        // Data merging service (used by background jobs for multi-provider ingestion)
        services.AddScoped<IProviderDataIngestionService, ProviderDataIngestionService>();

        // SuperSubtitles import config
        services.Configure<SuperSubtitlesImportConfig>(
            configuration.GetSection(SuperSubtitlesImportConfig.SectionName));
    }
}