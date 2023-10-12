#region

using System.Net;
using System.Net.Http.Headers;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Service.Exception;
using AddictedProxy.Upstream.Service.Performance;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Prometheus;

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
        return await Policy()
            .ExecuteAsync(async cancellationToken =>
            {
                var request = _httpUtils.PrepareRequest(credentials, subtitle.DownloadUri.ToString(), HttpMethod.Get);
                return await DownloadSubtitleFile(credentials, cancellationToken, request);
            }, token);
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

    private AsyncPolicy Policy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(500), retryCount: 3);
        return Polly.Policy.Handle<HttpRequestException>(exception => exception.InnerException is IOException)
                    .WaitAndRetryAsync(delay);
    }
}