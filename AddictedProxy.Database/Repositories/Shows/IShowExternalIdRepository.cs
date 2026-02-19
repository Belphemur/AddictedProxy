using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface IShowExternalIdRepository
{
    /// <summary>
    /// Get the show external ID for a given source and external ID
    /// </summary>
    Task<ShowExternalId?> GetBySourceAndExternalIdAsync(DataSource source, string externalId, CancellationToken token);

    /// <summary>
    /// Get all external IDs for a given show
    /// </summary>
    Task<IReadOnlyList<ShowExternalId>> GetByShowIdAsync(long tvShowId, CancellationToken token);

    /// <summary>
    /// Get the external ID for a specific source and show
    /// </summary>
    Task<ShowExternalId?> GetByShowAndSourceAsync(long tvShowId, DataSource source, CancellationToken token);

    /// <summary>
    /// Add or update a show external ID
    /// </summary>
    Task UpsertAsync(ShowExternalId showExternalId, CancellationToken token);

    /// <summary>
    /// Returns the subset of the provided external IDs that already exist for the given source.
    /// Use this for bulk existence checks instead of querying one by one.
    /// </summary>
    Task<IReadOnlySet<string>> GetExistingExternalIdsAsync(DataSource source, IEnumerable<string> externalIds, CancellationToken token);

    /// <summary>
    /// Add or update multiple show external IDs
    /// </summary>
    Task BulkUpsertAsync(IEnumerable<ShowExternalId> showExternalIds, CancellationToken token);
}
