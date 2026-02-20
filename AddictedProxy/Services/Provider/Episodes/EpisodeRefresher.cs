using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Episodes;

public class EpisodeRefresher : IEpisodeRefresher
{
    private readonly IEpisodeRepository _episodeRepository;
    private readonly IShowExternalIdRepository _showExternalIdRepository;
    private readonly ProviderEpisodeRefresherFactory _providerEpisodeRefresherFactory;
    private readonly ILogger<EpisodeRefresher> _logger;
    private readonly IPerformanceTracker _performanceTracker;

    public EpisodeRefresher(IEpisodeRepository episodeRepository,
                            IShowExternalIdRepository showExternalIdRepository,
                            ProviderEpisodeRefresherFactory providerEpisodeRefresherFactory,
                            ILogger<EpisodeRefresher> logger,
                            IPerformanceTracker performanceTracker)
    {
        _episodeRepository = episodeRepository;
        _showExternalIdRepository = showExternalIdRepository;
        _providerEpisodeRefresherFactory = providerEpisodeRefresherFactory;
        _logger = logger;
        _performanceTracker = performanceTracker;
    }

    /// <summary>
    /// Get episode. It might have been refreshed from any provider.
    /// </summary>
    public async Task<Episode?> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token)
    {
        var externalIds = await _showExternalIdRepository.GetByShowIdAsync(show.Id, token);

        foreach (var extId in externalIds)
        {
            var refresher = _providerEpisodeRefresherFactory.GetService(extId.Source);
            var result = await refresher.GetRefreshEpisodeAsync(show, extId, season, episodeNumber, token);
            if (result != null)
            {
                return result;
            }
        }

        // Final fallback: check DB in case episodes were already loaded by another path
        return await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);
    }

    /// <summary>
    /// Refresh subtitle of specific seasons of the show from all providers.
    /// </summary>
    public async Task RefreshEpisodesAsync(TvShow show, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token)
    {
        var externalIds = await _showExternalIdRepository.GetByShowIdAsync(show.Id, token);

        foreach (var extId in externalIds)
        {
            var refresher = _providerEpisodeRefresherFactory.GetService(extId.Source);
            await refresher.RefreshEpisodesAsync(show, extId, seasonsToRefresh, sendProgress, token);
        }
    }

    /// <summary>
    /// Does the episode of the season need to be refreshed from any provider.
    /// </summary>
    public bool IsSeasonNeedRefresh(TvShow show, Season season)
    {
        foreach (var refresher in _providerEpisodeRefresherFactory.Services)
        {
            if (refresher.IsSeasonNeedRefresh(show, season))
            {
                return true;
            }
        }

        return false;
    }
}