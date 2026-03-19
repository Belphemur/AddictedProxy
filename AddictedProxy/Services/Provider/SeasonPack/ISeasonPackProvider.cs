using AddictedProxy.Database.Model.Shows;
using Microsoft.AspNetCore.Http.HttpResults;

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
    Task<Results<FileStreamHttpResult, NotFound<string>>> GetSeasonPackZipAsync(Guid uniqueId, CancellationToken token);

    /// <summary>
    /// Self-extract a specific catalog entry from the stored ZIP
    /// </summary>
    Task<Results<FileStreamHttpResult, NotFound<string>>> GetEntryFileAsync(Guid seasonPackUniqueId, Guid entryUniqueId, CancellationToken token);

    /// <summary>
    /// Download a single episode SRT from upstream (fallback for non-cataloged packs)
    /// </summary>
    Task<Results<FileStreamHttpResult, NotFound<string>>> GetEpisodeFromUpstreamAsync(Guid seasonPackUniqueId, int episode, CancellationToken token);
}
