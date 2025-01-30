#region

using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Model;
using AddictedProxy.Upstream.Service.Exception;
using Microsoft.Extensions.Logging;

#endregion

namespace AddictedProxy.Upstream.Service;

internal class Addic7edClient : IAddic7edClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpUtils _httpUtils;
    private readonly ILogger<Addic7edClient> _logger;
    private readonly Parser _parser;

    public Addic7edClient(HttpClient httpClient, Parser parser, HttpUtils httpUtils, ILogger<Addic7edClient> logger)
    {
        _httpClient = httpClient;
        _parser = parser;
        _httpUtils = httpUtils;
        _logger = logger;
    }

    /// <summary>
    /// Get the download usage of a specific credential
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<DownloadUsage?> GetDownloadUsageAsync(AddictedUserCredentials credentials, CancellationToken token)
    {
        using var _ = _logger.BeginScope("Getting download usage for {CredId}", credentials.Id);
        using var response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(credentials, "panel.php", HttpMethod.Get), token);
        return await _parser.GetDownloadUsageAsync(await response.Content.ReadAsStreamAsync(token), token);
    }

    /// <summary>
    ///     Get Tv Shows
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<TvShow> GetTvShowsAsync(AddictedUserCredentials creds, [EnumeratorCancellation] CancellationToken token)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(creds, "ajax_getShows.php", HttpMethod.Get), token);
            var shows = _parser.GetShowsAsync(await response.Content.ReadAsStreamAsync(token), token);
            await foreach (var show in shows.WithCancellation(token))
            {
                yield return show;
            }
        }
        finally
        {
            response?.Dispose();
        }
    }

    /// <summary>
    /// Clean up the inbox for the given account
    /// </summary>
    /// <param name="creds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> CleanupInbox(AddictedUserCredentials creds, CancellationToken token)
    {
        var httpRequestMessage = _httpUtils.PrepareRequest(creds, "mailactions.php", HttpMethod.Post,
            new FormUrlEncodedContent(new Dictionary<string, string> { { "inboxdelall", "value" }, { "delall", "all" } }));
        using var response = await _httpClient.SendAsync(httpRequestMessage, token);
        return response.StatusCode is HttpStatusCode.Found;
    }

    /// <summary>
    ///     Get nb of season the show has
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Season>> GetSeasonsAsync(AddictedUserCredentials credentials, TvShow show, CancellationToken token)
    {
        try
        {
            using var response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(credentials, $"ajax_getSeasons.php?showID={show.ExternalId}", HttpMethod.Get), token);
            return await _parser.GetSeasonsAsync(await response.Content.ReadAsStreamAsync(token), token)
                .Select(i => new Season { Number = i, TvShowId = show.Id })
                .ToArrayAsync(token);
        }
        catch (NothingToParseException)
        {
            return Array.Empty<Season>();
        }
    }

    /// <summary>
    ///     Get episode for the following season
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="season"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Episode>> GetEpisodesAsync(AddictedUserCredentials credentials, TvShow show, int season, CancellationToken token)
    {
        using var scope = _logger.BeginScope("Getting episode for {Show} {Season}", show.Name, season);
        using var response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(credentials, $"ajax_loadShow.php?show={show.ExternalId}&season={season}&langs=&hd=undefined&hi=undefined", HttpMethod.Get),
            token);
        return await _parser.GetSeasonSubtitlesAsync(await response.Content.ReadAsStreamAsync(token), token)
            .Select(episode =>
            {
                episode.TvShowId = show.Id;
                return episode;
            })
            .ToArrayAsync(token);
    }
}