using System.Runtime.CompilerServices;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed.Exception;
using JetBrains.Annotations;
using MD5Hash;
using Microsoft.Extensions.Caching.Memory;
using Polly;

namespace AddictedProxy.Services.Addic7ed
{
    public class Addic7edClient : IAddic7edClient
    {
        private readonly HttpClient _httpClient;
        private readonly Parser _parser;

        public Addic7edClient(HttpClient httpClient, Parser parser)
        {
            _httpClient = httpClient;
            _parser = parser;
            _httpClient.BaseAddress = new Uri("https://www.addic7ed.com");
        }

        /// <summary>
        /// Get Tv Shows
        /// </summary>
        /// <param name="creds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<TvShow> GetTvShowsAsync(Addic7edCreds creds, [EnumeratorCancellation] CancellationToken token)
        {
            HttpResponseMessage? response = null;
            try
            {
                var shows = await Policy().ExecuteAsync(async cToken =>
                {
                    response = await _httpClient.SendAsync(PrepareRequest(creds, "ajax_getShows.php", HttpMethod.Get), cToken);
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
        /// Get nb of season the show has
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Season>> GetSeasonsAsync(Addic7edCreds credentials, TvShow show, CancellationToken token)
        {
            return await Policy().ExecuteAsync(async cToken =>
            {
                using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_getSeasons.php?showID={show.Id}", HttpMethod.Get), cToken);
                return await _parser.GetSeasonsAsync(await response.Content.ReadAsStreamAsync(cToken), cToken)
                    .Select(i => new Season { TvShow = show, Number = i, TvShowId = show.Id})
                    .ToArrayAsync(cToken);
            }, token);
        }

        /// <summary>
        /// Get episode for the following season
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="season"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Episode>> GetEpisodesAsync(Addic7edCreds credentials, TvShow show, int season, CancellationToken token)
        {
            return await Policy().ExecuteAsync(async cToken =>
            {
                using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_loadShow.php?bot=1&show={show.Id}&season={season}&langs=&hd=undefined&hi=undefined", HttpMethod.Get), token);
                return await _parser.GetSeasonSubtitlesAsync(await response.Content.ReadAsStreamAsync(cToken), cToken)
                    .Select(episode =>
                    {
                        episode.TvShow = show;
                        return episode;
                    })
                    .ToArrayAsync(cToken);
            }, token);
        }

        private AsyncPolicy Policy()
        {
            var jitterer = new Random();
            return Polly.Policy.Handle<NothingToParseException>()
                .WaitAndRetryAsync(8, // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 300))
                );
        }

        private HttpRequestMessage PrepareRequest(Addic7edCreds? credentials, string url, HttpMethod method)
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
}