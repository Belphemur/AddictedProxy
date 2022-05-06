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
    /// <param name="token"></param>
    Task RefreshSubtitlesAsync(TvShow show, Season season, CancellationToken token);

    /// <summary>
    /// Get episode. It might have been refreshed.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="episodeNumber"></param>
    /// <param name="timeBetweenChecks"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<(Episode? episode, bool episodesRefreshed)> GetRefreshEpisodeAsync(TvShow show, Season season, int episodeNumber, TimeSpan timeBetweenChecks, CancellationToken token);
}