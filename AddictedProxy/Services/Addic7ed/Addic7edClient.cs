using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Caching;
using JetBrains.Annotations;
using MD5Hash;

namespace AddictedProxy.Services.Addic7ed
{
    public class Addic7edClient : IAddic7edClient
    {
        private readonly HttpClient      _httpClient;
        private readonly Parser          _parser;
        private readonly ICachingService _cachingService;

        public Addic7edClient(HttpClient httpClient, Parser parser, ICachingService cachingService)
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
            return await _cachingService.GetSetAsync("shows", async cancellationToken =>
            {
                using var response = await _httpClient.SendAsync(PrepareRequest(credentials, "ajax_getShows.php", HttpMethod.Get), cancellationToken);
                var result =  await _parser.GetShowsAsync(await response.Content.ReadAsStreamAsync(), token).ToArrayAsync(cancellationToken);
                if (result.Length == 0)
                {
                    throw new Exception("Couldn't find shows");
                }

                return result;
            }, TimeSpan.FromDays(1), token);
        }

        /// <summary>
        /// Get nb of season the show has
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="show"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<int> GetNbSeasonsAsync(Addic7edCreds credentials, TvShow show, CancellationToken token)
        {
            return await _cachingService.GetSetAsync($"show-nb-season-{show.Id}", async cancellationToken =>
            {
                using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_getSeasons.php?showID={show.Id}", HttpMethod.Get), cancellationToken);
                return await _parser.GetNumberOfSeasonsAsync(await response.Content.ReadAsStreamAsync(), cancellationToken);
            }, TimeSpan.FromDays(1), token);
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
            return await _cachingService.GetSetAsync($"show-episodes-{show.Id}", async cancellationToken =>
            {
                using var response = await _httpClient.SendAsync(PrepareRequest(credentials, $"ajax_loadShow.php?bot=1&show={show.Id}&season={season}&langs=&hd=undefined&hi=undefined", HttpMethod.Get), token);
                return await _parser.GetSeasonSubtitlesAsync(await response.Content.ReadAsStreamAsync(), token).ToArrayAsync(token);
            }, TimeSpan.FromHours(1), token);
        }

        private HttpRequestMessage PrepareRequest([CanBeNull] Addic7edCreds credentials, string url, HttpMethod method)
        {
            if (credentials?.Password == null || credentials.UserId == 0)
            {
                return new HttpRequestMessage(method, url);
            }

            var md5Pass = Hash.Content(credentials.Password);
            return new HttpRequestMessage(method, url)
            {
                Headers = {{"Cookie", $"wikisubtitlespass={md5Pass};wikisubtitlesuser={credentials.UserId}"}}
            };
        }
    }
}