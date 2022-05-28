#region

using System.Net.Http.Headers;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Service.Exception;

#endregion

namespace AddictedProxy.Upstream.Service;

public class Addic7edDownloader : IAddic7edDownloader
{
    private static readonly MediaTypeHeaderValue ContentTypeHtml = new("text/html");
    private readonly HttpClient _httpClient;
    private readonly HttpUtils _httpUtils;

    public Addic7edDownloader(HttpClient httpClient, HttpUtils httpUtils)
    {
        _httpClient = httpClient;
        _httpUtils = httpUtils;
        _httpClient.BaseAddress = new Uri("https://www.addic7ed.com");
    }

    public Task<Stream> DownloadSubtitle(AddictedUserCredentials? credentials, Subtitle subtitle, CancellationToken token)
    {
        var request = _httpUtils.PrepareRequest(credentials, subtitle.DownloadUri.ToString(), HttpMethod.Get);
        return DownloadSubtitleFile(credentials, token, request);
    }

    private async Task<Stream> DownloadSubtitleFile(AddictedUserCredentials? credentials, CancellationToken token, HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request, token);
        if (!response.IsSuccessStatusCode || ContentTypeHtml.Equals(response.Content.Headers.ContentType))
        {
            throw new DownloadLimitExceededException($"Reached limit for download for {credentials?.Id}");
        }

        return await response.Content.ReadAsStreamAsync(token);
    }
}