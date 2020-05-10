using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers
{
    [ApiController]
    [Route("addic7ed")]
    public class Addic7ed : Controller
    {
        private readonly IAddic7edClient _client;

        public class SearchRequest
        {
            public Addic7edCreds Credentials { get; set; }
            public string        Show        { get; set; }
            public int           Episode     { get; set; }
            public int           Season      { get; set; }
            public string        FileName    { get; set; }
        }

        public class SearchResponse
        {
            public Subtitle[] MatchingSubtitles { get; set; }

            public Episode Episode { get; set; }
        }

        public Addic7ed(IAddic7edClient client)
        {
            _client = client;
        }

        [Route("shows")]
        [HttpPost]
        public async Task<IActionResult> GetShows([FromBody] Addic7edCreds credentials, CancellationToken token)
        {
            var shows = await _client.GetTvShowsAsync(credentials, token);
            return Ok(shows);
        }

        [Route("download/{lang:int}/{id:int}/{version:int}")]
        [HttpPost]
        public async Task<IActionResult> Search([FromBody] Addic7edCreds credentials, [FromRoute] int lang, [FromRoute] int id, [FromRoute] int version, CancellationToken token)
        {
            return Ok();
        }

        [Route("search")]
        [HttpPost]
        public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken token)
        {
            var shows = await _client.GetTvShowsAsync(request.Credentials, token);
            var show  = shows.FirstOrDefault(tvShow => string.Equals(tvShow.Name, request.Show, StringComparison.InvariantCultureIgnoreCase));
            if (show == null)
            {
                return NotFound(new {Error = $"Couldn't find the show {request.Show}"});
            }

            var season = await _client.GetNbSeasonsAsync(request.Credentials, show, token);
            if (season <= 0)
            {
                return NotFound(new {Error = $"Couldn't find a season for {request.Show}"});
            }

            if (season < request.Season)
            {
                return NotFound(new {Error = $"Couldn't find season ${request.Season} for {request.Show}"});
            }

            var episodes = await _client.GetEpisodesAsync(request.Credentials, show, request.Season, token);
            var episode  = episodes.FirstOrDefault(ep => ep.Number == request.Episode);
            if (episode == null)
            {
                return NotFound(new {Error = $"Couldn't find episode S{request.Season}E{request.Episode} for {request.Show}"});
            }

            var matchingSubtitles = episode.Subtitles
                                           .Where(subtitle => { return subtitle.Version.Split("+").Any(version => request.FileName.ToLowerInvariant().Contains(version.ToLowerInvariant())); })
                                           .ToArray();
            return Ok(new SearchResponse
            {
                Episode           = episode,
                MatchingSubtitles = matchingSubtitles
            });
        }
    }
}