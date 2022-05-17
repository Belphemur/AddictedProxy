using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Episodes;

public interface IEpisodeRefresher
{
    /// <summary>
    /// Refresh subtitle of a specific show and season
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="forceRefresh"></param>
    /// <param name="token"></param>
    Task RefreshEpisodesAsync(TvShow show, Season season, bool forceRefresh = false, CancellationToken token = default);

    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<(Episode? episode, bool episodesRefreshed)> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token);

    /// <summary>
    /// Refresh subtitle of specific seasons of the show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasonsToRefresh"></param>
    /// <param name="sendProgress"></param>
    /// <param name="token"></param>
    Task RefreshEpisodesAsync(TvShow show, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token);
}