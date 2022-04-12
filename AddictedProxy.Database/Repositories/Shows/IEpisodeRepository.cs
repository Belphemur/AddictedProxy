using AddictedProxy.Database.Model.Shows;

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
    Task<Episode?> GetEpisodeUntrackedAsync(int tvShowId, int season, int episodeNumber, CancellationToken token);
}