#region

using System.Runtime.CompilerServices;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Upstream.Service.Exception;
using Polly;
using Polly.Contrib.WaitAndRetry;

#endregion

namespace AddictedProxy.Upstream.Service;

public class Addic7edClient : IAddic7edClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpUtils _httpUtils;
    private readonly Parser _parser;

    public Addic7edClient(HttpClient httpClient, Parser parser, HttpUtils httpUtils)
    {
        _httpClient = httpClient;
        _parser = parser;
        _httpUtils = httpUtils;
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
            var shows = await Policy()
                .ExecuteAsync(async cToken =>
                {
                    response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(creds, "ajax_getShows.php", HttpMethod.Get), cToken);
                    return _parser.GetShowsAsync(await response.Content.ReadAsStreamAsync(cToken), cToken);
                }, token);
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
    ///     Get nb of season the show has
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Season>> GetSeasonsAsync(AddictedUserCredentials credentials, TvShow show, CancellationToken token)
    {
        return await Policy()
            .ExecuteAsync(async cToken =>
            {
                using var response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(credentials, $"ajax_getSeasons.php?showID={show.ExternalId}", HttpMethod.Get), cToken);
                return await _parser.GetSeasonsAsync(await response.Content.ReadAsStreamAsync(cToken), cToken)
                                    .Select(i => new Season { Number = i, TvShowId = show.Id })
                                    .ToArrayAsync(cToken);
            }, token);
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
        return await Policy()
            .ExecuteAsync(async cToken =>
            {
                using var response = await _httpClient.SendAsync(_httpUtils.PrepareRequest(credentials, $"ajax_loadShow.php?bot=1&show={show.ExternalId}&season={season}&langs=&hd=undefined&hi=undefined", HttpMethod.Get),
                    token);
                return await _parser.GetSeasonSubtitlesAsync(await response.Content.ReadAsStreamAsync(cToken), cToken)
                                    .Select(episode =>
                                    {
                                        episode.TvShowId = show.Id;
                                        return episode;
                                    })
                                    .ToArrayAsync(cToken);
            }, token);
    }

    private AsyncPolicy Policy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(500), retryCount: 8);
        return Polly.Policy.Handle<NothingToParseException>()
                    .WaitAndRetryAsync(delay);
    }
}