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
    ///     Get a specific episode
    /// </summary>
    Task<Episode?> GetEpisodeUntrackedAsync(long tvShowId, int season, int episodeNumber, CancellationToken token);
}