using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database.Repositories;

public interface IEpisodeRepository
{
    /// <summary>
    /// Upsert the episodes
    /// </summary>
    Task UpsertEpisodes(IEnumerable<Episode> episodes, CancellationToken token);

    /// <summary>
    /// Get a specific episode
    /// </summary>
    Task<Episode?> GetEpisodeAsync(int tvShowId, int season, int episodeNumber, CancellationToken token);
}