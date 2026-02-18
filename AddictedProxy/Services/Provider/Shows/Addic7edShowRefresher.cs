using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows.Hub;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// Addic7ed-specific show refresh: fetches seasons and episodes from the Addic7ed API.
/// Uses the provider-specific season and episode refreshers via their factories.
/// </summary>
internal class Addic7edShowRefresher : IProviderShowRefresher
{
    private readonly ProviderSeasonRefresherFactory _seasonRefresherFactory;
    private readonly ProviderEpisodeRefresherFactory _episodeRefresherFactory;
    private readonly IRefreshHubManager _refreshHubManager;
    private readonly ILogger<Addic7edShowRefresher> _logger;

    public Addic7edShowRefresher(ProviderSeasonRefresherFactory seasonRefresherFactory,
                                 ProviderEpisodeRefresherFactory episodeRefresherFactory,
                                 IRefreshHubManager refreshHubManager,
                                 ILogger<Addic7edShowRefresher> logger)
    {
        _seasonRefresherFactory = seasonRefresherFactory;
        _episodeRefresherFactory = episodeRefresherFactory;
        _refreshHubManager = refreshHubManager;
        _logger = logger;
    }

    public DataSource Enum => DataSource.Addic7ed;

    public bool IsShowNeedsRefresh(TvShow show)
    {
        return _seasonRefresherFactory.GetService(DataSource.Addic7ed).IsShowNeedsRefresh(show);
    }

    public async Task RefreshShowAsync(TvShow show, ShowExternalId externalId, CancellationToken token)
    {
        var seasonRefresher = _seasonRefresherFactory.GetService(DataSource.Addic7ed);
        await seasonRefresher.RefreshSeasonsAsync(show, externalId, token);

        await _refreshHubManager.SendProgressAsync(show, 25, token);

        var seasonToSync = show.Seasons.OrderByDescending(season => season.Number).ToArray();

        _logger.LogInformation("Refreshing episodes for {number} seasons of {show} (Addic7ed)", seasonToSync.Length, show.Name);

        var progressMin = 25;
        var progressMax = 100;

        async Task SendProgress(int progress)
        {
            var refreshValue = Convert.ToInt32(Math.Ceiling(progressMin + (progressMax - progressMin) * progress / 100.0));
            await _refreshHubManager.SendProgressAsync(show, refreshValue, token);
        }

        var episodeRefresher = _episodeRefresherFactory.GetService(DataSource.Addic7ed);
        await episodeRefresher.RefreshEpisodesAsync(show, externalId, seasonToSync, SendProgress, token);
    }
}
