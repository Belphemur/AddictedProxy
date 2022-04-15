#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Service.Exception;

#endregion

namespace AddictedProxy.Upstream.Service;

public interface IAddic7edDownloader
{
    /// <summary>
    ///     Download a specific subtitle
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <exception cref="DownloadLimitExceededException">When we reach limit in Addicted to download the subtitle</exception>
    /// <returns></returns>
    Task<Stream> DownloadSubtitle(AddictedUserCredentials credentials, Subtitle subtitle, CancellationToken token);
}