using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface IEpisodeExternalIdRepository
{
    /// <summary>
    /// Get the episode external ID for a given source and external ID
    /// </summary>
    Task<EpisodeExternalId?> GetBySourceAndExternalIdAsync(DataSource source, string externalId, CancellationToken token);

    /// <summary>
    /// Get all external IDs for a given episode
    /// </summary>
    Task<IReadOnlyList<EpisodeExternalId>> GetByEpisodeIdAsync(long episodeId, CancellationToken token);

    /// <summary>
    /// Add or update an episode external ID
    /// </summary>
    Task UpsertAsync(EpisodeExternalId episodeExternalId, CancellationToken token);

    /// <summary>
    /// Add or update multiple episode external IDs
    /// </summary>
    Task BulkUpsertAsync(IEnumerable<EpisodeExternalId> episodeExternalIds, CancellationToken token);
}
