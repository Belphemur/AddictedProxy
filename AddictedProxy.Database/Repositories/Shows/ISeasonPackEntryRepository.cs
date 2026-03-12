using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISeasonPackEntryRepository
{
    /// <summary>
    /// Bulk upsert season pack entries (matched on SeasonPackSubtitleId + FileName)
    /// </summary>
    Task BulkUpsertAsync(IEnumerable<SeasonPackEntry> entries, CancellationToken token);

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
}
