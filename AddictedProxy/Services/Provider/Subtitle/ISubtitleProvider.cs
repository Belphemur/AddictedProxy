#region

using AddictedProxy.Database.Model.Shows;
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
    /// Get the <see cref="Subtitle"/> with its <see cref="Episode"/> and attached <see cref="TvShow"/>
    /// </summary>
    /// <param name="subtitleId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<Database.Model.Shows.Subtitle?> GetSubtitleFullAsync(Guid subtitleId, CancellationToken token);
}