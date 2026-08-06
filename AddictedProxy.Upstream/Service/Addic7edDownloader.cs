#region

using System.Net;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Service.Exception;
using AddictedProxy.Upstream.Service.Performance;

#endregion

namespace AddictedProxy.Upstream.Service;

internal class Addic7edDownloader : IAddic7edDownloader
{
    private readonly HttpClient _httpClient;
    private readonly HttpUtils _httpUtils;
    private readonly DownloadCounterWrapper _downloadCounterWrapper;

    public Addic7edDownloader(HttpClient httpClient, HttpUtils httpUtils, DownloadCounterWrapper downloadCounterWrapper)
    {
        _httpClient = httpClient;
        _httpUtils = httpUtils;
        _downloadCounterWrapper = downloadCounterWrapper;
    }

    public async Task<Stream> DownloadSubtitle(AddictedUserCredentials? credentials, Subtitle subtitle, CancellationToken token)
    {
        var request = _httpUtils.PrepareRequest(credentials, subtitle.DownloadUri.ToString(), HttpMethod.Get);
        return await DownloadSubtitleFile(credentials, token, request);
    }

    private async Task<Stream> DownloadSubtitleFile(AddictedUserCredentials? credentials, CancellationToken cancellationToken, HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Redirect && response.Headers.Location != null)
        {
            var path = response.Headers.Location.ToString();

            if (path.StartsWith("/downloadexceeded.php"))
            {
                _downloadCounterWrapper.Inc(DownloadCounterWrapper.SubtitleRequestResult.DownloadLimitReached);
                throw new DownloadLimitExceededException($"Reached limit for download for {credentials?.Id}");
            }

            _downloadCounterWrapper.Inc(DownloadCounterWrapper.SubtitleRequestResult.Deleted);
            throw new SubtitleFileDeletedException($"File deleted at location: {request.RequestUri}");
        }

        _downloadCounterWrapper.Inc(DownloadCounterWrapper.SubtitleRequestResult.Downloaded);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
