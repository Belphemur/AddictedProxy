using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.SeasonPack;

public interface ISeasonPackProvider
{
    /// <summary>
    /// Get the season pack subtitle with its navigation properties loaded
    /// </summary>
    Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token);

    /// <summary>
    /// Get a season pack entry by its unique ID
    /// </summary>
    Task<SeasonPackEntry?> GetEntryByUniqueIdAsync(Guid uniqueId, CancellationToken token);

    /// <summary>
    /// Download the full season pack ZIP archive
    /// </summary>
    Task<Stream> GetSeasonPackZipAsync(SeasonPackSubtitle seasonPack, CancellationToken token);

    /// <summary>
    /// Self-extract a specific catalog entry from the stored ZIP
    /// </summary>
    Task<Stream> GetEntryFileAsync(SeasonPackSubtitle seasonPack, SeasonPackEntry entry, CancellationToken token);

    /// <summary>
    /// Download a single episode SRT from upstream (fallback for non-cataloged packs)
    /// </summary>
    Task<Stream> GetEpisodeFromUpstreamAsync(SeasonPackSubtitle seasonPack, int episode, CancellationToken token);
}
