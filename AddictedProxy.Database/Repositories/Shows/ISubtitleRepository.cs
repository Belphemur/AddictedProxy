using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISubtitleRepository
{
    /// <summary>
    ///     Get Subtitle by their Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Subtitle?> GetSubtitleByIdAsync(int id, CancellationToken token);

    /// <summary>
    ///     Get Subtitle by their Id
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="withEpisode"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Subtitle?> GetSubtitleByGuidAsync(Guid uniqueId, bool withEpisode = false, CancellationToken token = default);

    /// <summary>
    /// Update the subtitle
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task UpdateAsync(Subtitle subtitle, CancellationToken token);
}