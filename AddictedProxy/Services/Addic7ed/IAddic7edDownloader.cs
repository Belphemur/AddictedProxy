using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed.Exception;

namespace AddictedProxy.Services.Addic7ed;

public interface IAddic7edDownloader
{
    /// <summary>
    /// Download the given subtitle
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="lang"></param>
    /// <param name="id"></param>
    /// <param name="version"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="DownloadLimitExceededException"> When can't download subtitle anymore</exception>
    Task<Stream> DownloadSubtitle(Addic7edCreds credentials, int lang, int id, int version, CancellationToken token);

    /// <summary>
    /// Download a specific subtitle
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Stream> DownloadSubtitle(Addic7edCreds credentials, Subtitle subtitle, CancellationToken token);
}