using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISeasonPackEntryRepository
{
    /// <summary>
    /// Synchronize all entries for a season pack so removed ZIP entries are deleted.
    /// </summary>
    Task BulkSyncAsync(long seasonPackSubtitleId, IEnumerable<SeasonPackEntry> entries, CancellationToken token);

    /// <summary>
    /// Check whether any entries exist for the given season pack
    /// </summary>
    Task<bool> HasEntriesAsync(long seasonPackSubtitleId, CancellationToken token);

    /// <summary>
    /// Get all entries for a season pack
    /// </summary>
    Task<IReadOnlyList<SeasonPackEntry>> GetBySeasonPackAsync(long seasonPackSubtitleId, CancellationToken token);

    /// <summary>
    /// Check whether a specific episode exists in a season pack
    /// </summary>
    Task<bool> HasEpisodeAsync(long seasonPackSubtitleId, int episodeNumber, CancellationToken token);

    /// <summary>
    /// Find a single entry by its unique ID
    /// </summary>
    Task<SeasonPackEntry?> FindByUniqueIdAsync(Guid uniqueId, CancellationToken token);

    /// <summary>
    /// Find all entries for a given episode number in a season pack
    /// </summary>
    Task<IReadOnlyList<SeasonPackEntry>> FindAllByEpisodeNumberAsync(long seasonPackSubtitleId, int episodeNumber, CancellationToken token);
}
