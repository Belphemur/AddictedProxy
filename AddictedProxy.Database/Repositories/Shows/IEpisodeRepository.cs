#region

using AddictedProxy.Database.Model.Shows;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public interface IEpisodeRepository
{
    /// <summary>
    ///     Upsert the episodes
    /// </summary>
    Task UpsertEpisodes(IEnumerable<Episode> episodes, CancellationToken token);

    /// <summary>
    /// Atomically upsert a single episode and its subtitle via SQL.
    /// Returns the database-generated episode ID.
    /// </summary>
    Task<long> MergeEpisodeWithSubtitleAsync(Episode episode, Subtitle subtitle, CancellationToken token);

    /// <summary>
    ///     Get a specific episode
    /// </summary>
    Task<Episode?> GetEpisodeUntrackedAsync(long tvShowId, int season, int episodeNumber, CancellationToken token);

    /// <summary>
    /// Get season episodes
    /// </summary>
    /// <param name="tvShowId"></param>
    /// <param name="season"></param>
    /// <returns></returns>
    IAsyncEnumerable<Episode> GetSeasonEpisodesAsync(long tvShowId, int season);

    /// <summary>
    /// Get season episodes for language
    /// </summary>
    /// <param name="tvShowId"></param>
    /// <param name="language"></param>
    /// <param name="season"></param>
    /// <returns></returns>
    IAsyncEnumerable<Episode> GetSeasonEpisodesByLangUntrackedAsync(long tvShowId, Culture.Model.Culture language, int season);
}