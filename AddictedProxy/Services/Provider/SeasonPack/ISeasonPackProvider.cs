using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.SeasonPack;

public interface ISeasonPackProvider
{
    /// <summary>
    /// Get the season pack subtitle with its navigation properties loaded
    /// </summary>
    /// <param name="uniqueId">UniqueId of the season pack</param>
    /// <param name="token"></param>
    /// <returns>The season pack subtitle, or null if not found</returns>
    Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token);

    /// <summary>
    /// Get the season pack file stream. Returns the full ZIP when episode is null,
    /// or a single episode SRT when episode is specified.
    /// </summary>
    /// <param name="seasonPack">The season pack subtitle entity</param>
    /// <param name="episode">If specified, extract only this episode number as SRT; otherwise return full ZIP</param>
    /// <param name="token"></param>
    /// <returns>The file stream</returns>
    Task<Stream> GetSeasonPackFileAsync(SeasonPackSubtitle seasonPack, int? episode, CancellationToken token);
}
