using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed.Exception;
using JetBrains.Annotations;
using MD5Hash;
using Microsoft.Extensions.Caching.Memory;
using Polly;

namespace AddictedProxy.Services.Addic7ed
{
    public class Addic7edClient : IAddic7edClient
    {
        private readonly HttpClient      _httpClient;
        private readonly Parser          _parser;
        private readonly IMemoryCache _cachingService;

        public Addic7edClient(HttpClient httpClient, Parser parser, IMemoryCache cachingService)
        {
            _httpClient             = httpClient;
            _parser                 = parser;
            _cachingService         = cachingService;
            _httpClient.BaseAddress = new Uri("https://www.addic7ed.com");
        }

        /// <summary>
        /// Get Tv Shows
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TvShow>> GetTvShowsAsync(Addic7edCreds credentials, CancellationToken token)
        {
            return await _cachingService.GetOrCreateAsync("shows", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return await Policy().ExecuteAsync(async cToken =>
                {
                    using var response = await _httpClient.SendAsync(PrepareRequest(credentials, "ajax_getShows.php", HttpMethod.Get), cToken);
                    return await _parser.GetShowsAsync(await response.Content.ReadAsStreamAsync(), token).ToArrayAsync(cToken);
                }, token);
            });
        }

        /// <summary>
        /// Get nb of season the show has
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetSeasonsAsync(Addic7edCreds credentials, TvShow show, CancellationToken token)
        {
            return await _cachingService.GetOrCreateAsync($"show-nb-season-{show.Id}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return await Policy().ExecuteAsync(async cToken =>
                {
                    using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_getSeasons.php?showID={show.Id}", HttpMethod.Get), cToken);
                    return await _parser.GetSeasonsAsync(await response.Content.ReadAsStreamAsync(), cToken).ToArrayAsync(cToken);
                }, token);
            });
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
            return await _cachingService.GetOrCreateAsync($"show-episodes-{show.Id}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await Policy().ExecuteAsync(async cToken =>
                {
                    using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_loadShow.php?bot=1&show={show.Id}&season={season}&langs=&hd=undefined&hi=undefined", HttpMethod.Get), token);
                    return await _parser.GetSeasonSubtitlesAsync(await response.Content.ReadAsStreamAsync(), token).ToArrayAsync(token);
                }, token);
            });
        }

        /// <summary>
        /// Download the given subtitle
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="lang"></param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadSubtitle(Addic7edCreds credentials, int lang, int id, int version, CancellationToken token)
        {
            var request = PrepareRequest(credentials, $"updated/{lang}/{id}/{version}", HttpMethod.Get);
            var response = await _httpClient.SendAsync(request, token);

            return await response.Content.ReadAsStreamAsync();
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

        private HttpRequestMessage PrepareRequest([CanBeNull] Addic7edCreds credentials, string url, HttpMethod method)
        {
            var request = new HttpRequestMessage(method, url)
            {
                Headers = {{"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0"}}
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