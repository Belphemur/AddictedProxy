using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Episodes;

public interface IEpisodeRefresher
{
    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Episode?> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, CancellationToken token);

    /// <summary>
    /// Refresh subtitle of specific seasons of the show
    /// </summary>
    /// <param name="show"></param>
    /// <param name="seasonsToRefresh"></param>
    /// <param name="sendProgress"></param>
    /// <param name="token"></param>
    Task RefreshEpisodesAsync(TvShow show, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token);

    /// <summary>
    /// Does the episode of the season need to be refreshed from any registered provider
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> IsSeasonNeedRefreshAsync(TvShow show, Season season, CancellationToken token);
}