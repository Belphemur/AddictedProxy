#region

using AddictedProxy.Database.Model.Shows;

#endregion

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
    /// <param name="withShow"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Subtitle?> GetSubtitleByGuidAsync(Guid uniqueId, bool withEpisode = false, bool withShow = false, CancellationToken token = default);

    /// <summary>
    /// Save any change in the database
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SaveChangeAsync(CancellationToken token);

    /// <summary>
    /// Tag subtitle for removal
    /// </summary>
    /// <param name="subtitle"></param>
    void TagForRemoval(Subtitle subtitle);
}