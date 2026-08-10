using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// Addic7ed-specific show refresh: fetches seasons and episodes from the Addic7ed API.
/// Uses the provider-specific season and episode refreshers via their factories.
/// </summary>
internal class Addic7edShowRefresher : IProviderShowRefresher
{
    private readonly ProviderSeasonRefresherFactory _seasonRefresherFactory;
    private readonly ProviderEpisodeRefresherFactory _episodeRefresherFactory;
    private readonly ILogger<Addic7edShowRefresher> _logger;

    public Addic7edShowRefresher(ProviderSeasonRefresherFactory seasonRefresherFactory,
                                 ProviderEpisodeRefresherFactory episodeRefresherFactory,
                                 ILogger<Addic7edShowRefresher> logger)
    {
        _seasonRefresherFactory = seasonRefresherFactory;
        _episodeRefresherFactory = episodeRefresherFactory;
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

        var seasonToSync = show.Seasons.OrderByDescending(season => season.Number).ToArray();

        _logger.LogInformation("Refreshing episodes for {number} seasons of {show} (Addic7ed)", seasonToSync.Length, show.Name);

        var episodeRefresher = _episodeRefresherFactory.GetService(DataSource.Addic7ed);
        await episodeRefresher.RefreshEpisodesAsync(externalId, seasonToSync, _ => Task.CompletedTask, token);
    }
}
