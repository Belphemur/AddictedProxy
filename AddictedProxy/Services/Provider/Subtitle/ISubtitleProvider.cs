#region

using AddictedProxy.Upstream.Service.Exception;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle;

public interface ISubtitleProvider
{
    /// <summary>
    /// Get the subtitle file stream
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <exception cref="DownloadLimitExceededException">When we reach limit in Addicted to download the subtitle</exception>
    /// <returns></returns>
    public Task<Stream> GetSubtitleFileAsync(Database.Model.Shows.Subtitle subtitle, CancellationToken token);

    /// <summary>
    /// Get the subtitle
    /// </summary>
    /// <param name="subtitleId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<Database.Model.Shows.Subtitle?> GetSubtitleAsync(Guid subtitleId, CancellationToken token);
}