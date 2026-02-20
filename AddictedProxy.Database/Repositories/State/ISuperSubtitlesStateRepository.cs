namespace AddictedProxy.Database.Repositories.State;

/// <summary>
/// Repository for tracking the SuperSubtitles sync cursor (max subtitle ID).
/// </summary>
public interface ISuperSubtitlesStateRepository
{
    /// <summary>
    /// Get the current max subtitle ID cursor.
    /// Returns 0 if no state has been stored yet.
    /// </summary>
    Task<long> GetMaxSubtitleIdAsync(CancellationToken token);

    /// <summary>
    /// Store the max subtitle ID cursor for incremental updates.
    /// </summary>
    Task SetMaxSubtitleIdAsync(long maxSubtitleId, CancellationToken token);
}
