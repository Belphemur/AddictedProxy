using System.Net.Http.Headers;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed.Exception;
using MD5Hash;

namespace AddictedProxy.Services.Addic7ed;

public class Addic7edDownloader : IAddic7edDownloader
{
    private static readonly MediaTypeHeaderValue ContentTypeHtml = new("text/html");
    private readonly HttpClient _httpClient;

    public Addic7edDownloader(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://www.addic7ed.com");
    }

    /// <summary>
    ///     Download the given subtitle
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="lang"></param>
    /// <param name="id"></param>
    /// <param name="version"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="DownloadLimitExceededException"> When can't download subtitle anymore</exception>
    public Task<Stream> DownloadSubtitle(Addic7edCreds credentials, int lang, int id, int version, CancellationToken token)
    {
        var request = PrepareRequest(credentials, new Uri($"updated/{lang}/{id}/{version}"), HttpMethod.Get);
        return DownloadSubtitleFile(credentials, token, request);
    }

    public Task<Stream> DownloadSubtitle(Addic7edCreds credentials, Subtitle subtitle, CancellationToken token)
    {
        var request = PrepareRequest(credentials, subtitle.DownloadUri, HttpMethod.Get);
        return DownloadSubtitleFile(credentials, token, request);
    }

    private async Task<Stream> DownloadSubtitleFile(Addic7edCreds credentials, CancellationToken token, HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request, token);
        if (!response.IsSuccessStatusCode || ContentTypeHtml.Equals(response.Content.Headers.ContentType))
        {
            throw new DownloadLimitExceededException($"Reached limit for download for {credentials.UserId}");
        }

        return await response.Content.ReadAsStreamAsync(token);
    }

    private HttpRequestMessage PrepareRequest(Addic7edCreds? credentials, Uri url, HttpMethod method)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Headers = { { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0" } }
        };
        if (credentials?.Password == null || credentials.UserId == 0)
        {
            return request;
        }

        var md5Pass = Hash.Content(credentials.Password);

        request.Headers.Add("Cookie", $"wikisubtitlespass={md5Pass};wikisubtitlesuser={credentials.UserId}");
        return request;
    }
}