#region

using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;

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
    /// <returns></returns>
    Task<Stream> DownloadSubtitle(AddictedUserCredentials credentials, Subtitle subtitle, CancellationToken token);
}