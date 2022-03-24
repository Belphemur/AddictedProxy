using AddictedProxy.Database;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Saver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AddictedProxy.Controllers
{
    [ApiController]
    [Route("addic7ed")]
    public class Addic7ed : Controller
    {
        private readonly IAddic7edClient _client;
        private readonly IAddictedSaver _addictedSaver;
        private readonly ITvShowRepository _tvShowRepository;
        private readonly ISeasonRepository _seasonRepository;

        public class SearchRequest
        {
            public Addic7edCreds Credentials { get; set; }
            public string Show { get; set; }
            public int Episode { get; set; }
            public int Season { get; set; }
            public string FileName { get; set; }
        }

        public class SearchResponse
        {
            public Subtitle[] MatchingSubtitles { get; set; }

            public Episode Episode { get; set; }
        }

        public Addic7ed(IAddic7edClient client, IAddictedSaver addictedSaver, ITvShowRepository tvShowRepository, ISeasonRepository seasonRepository)
        {
            _client = client;
            _addictedSaver = addictedSaver;
            _tvShowRepository = tvShowRepository;
            _seasonRepository = seasonRepository;
        }

        [Route("download/{lang:int}/{id:int}/{version:int}")]
        [HttpPost]
        public async Task<IActionResult> Download([FromBody] Addic7edCreds credentials, [FromRoute] int lang, [FromRoute] int id, [FromRoute] int version, CancellationToken token)
        {
            var subtitleStream = await _client.DownloadSubtitle(credentials, lang, id, version, token);
            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"));
        }

        [Route("search")]
        [HttpPost]
        public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken token)
        {
            var show = await _tvShowRepository.FindAsync(request.Show).FirstOrDefaultAsync(token);
            if (show == null)
            {
                return NotFound(new { Error = $"Couldn't find the show {request.Show}" });
            }

            var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);

            if (season == null && (DateTime.UtcNow - show.LastSeasonRefreshed) >= TimeSpan.FromMinutes(30))
            {
                var seasons = (await _client.GetSeasonsAsync(request.Credentials, show, token)).ToArray();
                await _seasonRepository.UpsertSeason(seasons, token);
                show.LastSeasonRefreshed = DateTime.UtcNow;
                await _tvShowRepository.UpdateShow(show, token);
                season = seasons.FirstOrDefault(s => s.Number == request.Season);
               
            }
            if (season == null)
            {
                return NotFound(new { Error = $"Couldn't find episode S{request.Season}E{request.Episode} for {request.Show}" });
            }
            


            // if (!season.Contains(request.Season))
            // {
            //     return NotFound(new { Error = $"Couldn't find season ${request.Season} for {request.Show}" });
            // }

            var episodes = await _client.GetEpisodesAsync(request.Credentials, show, request.Season, token);
            var episode = episodes.FirstOrDefault(ep => ep.Number == request.Episode);
            if (episode == null)
            {
                return NotFound(new { Error = $"Couldn't find episode S{request.Season}E{request.Episode} for {request.Show}" });
            }

            var matchingSubtitles = episode.Subtitles
                                           .Where(subtitle => !string.IsNullOrEmpty(subtitle.Version))
                                           .Where(subtitle => { return subtitle.Version.Split("+").Any(version => request.FileName.ToLowerInvariant().Contains(version.ToLowerInvariant())); })
                                           .ToArray();
            return Ok(new SearchResponse
            {
                Episode = episode,
                MatchingSubtitles = matchingSubtitles
            });
        }
    }
}