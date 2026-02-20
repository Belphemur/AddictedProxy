using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Episodes;

/// <summary>
/// Provider-specific episode refresh logic, keyed by <see cref="DataSource"/>.
/// Each provider implements this to handle its own episode refresh mechanism.
/// </summary>
public interface IProviderEpisodeRefresher : IEnumService<DataSource>
{
    /// <summary>
    /// Get an episode, refreshing from the provider if needed.
    /// </summary>
    /// <param name="show">The show.</param>
    /// <param name="showExternalId">The provider-specific external ID for the show.</param>
    /// <param name="season">The season containing the episode.</param>
    /// <param name="episodeNumber">The episode number to find.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The episode if found after refresh, null otherwise.</returns>
    Task<Episode?> GetRefreshEpisodeAsync(TvShow show, ShowExternalId showExternalId, Season season, int episodeNumber, CancellationToken token);

    /// <summary>
    /// Refresh episodes for specific seasons of the show from this provider.
    /// </summary>
    /// <param name="show">The show.</param>
    /// <param name="showExternalId">The provider-specific external ID for the show.</param>
    /// <param name="seasonsToRefresh">The seasons to refresh episodes for.</param>
    /// <param name="sendProgress">Callback to report progress (0-100).</param>
    /// <param name="token">Cancellation token.</param>
    Task RefreshEpisodesAsync(TvShow show, ShowExternalId showExternalId, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token);

    /// <summary>
    /// Whether episodes of the season need to be refreshed from this provider.
    /// </summary>
    bool IsSeasonNeedRefresh(TvShow show, Season season);
}
