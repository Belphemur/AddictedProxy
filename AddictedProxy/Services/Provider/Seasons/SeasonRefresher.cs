using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Seasons;

public class SeasonRefresher : ISeasonRefresher
{
    private readonly ILogger<SeasonRefresher> _logger;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IShowExternalIdRepository _showExternalIdRepository;
    private readonly ProviderSeasonRefresherFactory _providerSeasonRefresherFactory;
    private readonly IPerformanceTracker _performanceTracker;

    public SeasonRefresher(ILogger<SeasonRefresher> logger,
                           ISeasonRepository seasonRepository,
                           IShowExternalIdRepository showExternalIdRepository,
                           ProviderSeasonRefresherFactory providerSeasonRefresherFactory,
                           IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _seasonRepository = seasonRepository;
        _showExternalIdRepository = showExternalIdRepository;
        _providerSeasonRefresherFactory = providerSeasonRefresherFactory;
        _performanceTracker = performanceTracker;
    }

    public async Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, CancellationToken token)
    {
        // Check first in the show seasons if we can find it
        var season = await _seasonRepository.GetSeasonForShowAsync(show.Id, seasonNumber, token);
        if (season != null)
        {
            return season;
        }

        // Season not found — try refreshing from all providers
        await RefreshSeasonsAsync(show, token);
        return await _seasonRepository.GetSeasonForShowAsync(show.Id, seasonNumber, token);
    }

    public async Task RefreshSeasonsAsync(TvShow show, CancellationToken token = default)
    {
        using var span = _performanceTracker.BeginNestedSpan("season", $"refresh-show-seasons for show {show.Name}");

        var externalIds = await _showExternalIdRepository.GetByShowIdAsync(show.Id, token);

        foreach (var extId in externalIds)
        {
            var refresher = _providerSeasonRefresherFactory.GetService(extId.Source);
            await refresher.RefreshSeasonsAsync(show, extId, token);
        }
    }

    /// <summary>
    /// Does the show need to have its seasons refreshed from any provider
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    public bool IsShowNeedsRefresh(TvShow show)
    {
        foreach (var refresher in _providerSeasonRefresherFactory.Services)
        {
            if (refresher.IsShowNeedsRefresh(show))
            {
                return true;
            }
        }

        return false;
    }
}