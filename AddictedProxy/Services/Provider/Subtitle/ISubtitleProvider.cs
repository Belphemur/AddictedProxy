using AddictedProxy.Upstream.Service.Exception;

namespace AddictedProxy.Services.Provider.Subtitle;

public interface ISubtitleProvider
{
    /// <summary>
    /// Get the subtitle file stream
    /// </summary>
    /// <param name="subtitleUniqueId"></param>
    /// <param name="token"></param>
    /// <exception cref="DownloadLimitExceededException">When we reach limit in Addicted to download the subtitle</exception>
    /// <returns></returns>
    Task<Stream?> GetSubtitleFileStreamAsync(Guid subtitleUniqueId, CancellationToken token);
}