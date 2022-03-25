using AddictedProxy.Database;
using AddictedProxy.Database.Repositories;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Addic7ed.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AddictedProxy.Controllers
{
    [ApiController]
    [Route("addic7ed")]
    public class Addic7ed : Controller
    {
        private readonly IAddic7edClient _client;
        private readonly IAddic7edDownloader _downloader;
        private readonly ITvShowRepository _tvShowRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly TimeSpan _timeBetweenChecks;

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
            public class SubtitleDto
            {
                public string Version { get; }
                public bool Completed { get; }
                public bool HearingImpaired { get; }
                public bool Corrected { get; }
                public bool HD { get; }
                public Uri DownloadUri { get; }
                public string Language { get; }

                /// <summary>
                /// When was the subtitle discovered
                /// </summary>
                public DateTime Discovered { get; }

                public SubtitleDto(Subtitle subtitle)
                {
                    Version = subtitle.Version;
                    Completed = subtitle.Completed;
                    HearingImpaired = subtitle.HearingImpaired;
                    HD = subtitle.HD;
                    Corrected = subtitle.Completed;
                    DownloadUri = subtitle.DownloadUri;
                    Language = subtitle.Language;
                    Discovered = subtitle.Discovered;
                }
            }

            public class EpisodeDto
            {
                public int Season { get; }
                public int Number { get; }
                public string Title { get; }

                /// <summary>
                /// When was the subtitle discovered
                /// </summary>
                public DateTime Discovered { get; }

                public EpisodeDto(Episode episode)
                {
                    Season = episode.Season;
                    Number = episode.Number;
                    Title = episode.Title;
                    Discovered = episode.Discovered;
                }
            }

            public IEnumerable<SubtitleDto> MatchingSubtitles { get; set; }

            public EpisodeDto Episode { get; set; }
        }

        public Addic7ed(IAddic7edClient client, IAddic7edDownloader downloader, ITvShowRepository tvShowRepository, ISeasonRepository seasonRepository, IEpisodeRepository episodeRepository)
        {
            _client = client;
            _downloader = downloader;
            _tvShowRepository = tvShowRepository;
            _seasonRepository = seasonRepository;
            _episodeRepository = episodeRepository;
            _timeBetweenChecks = TimeSpan.FromMinutes(30);
        }

        [Route("download/{lang:int}/{id:int}/{version:int}")]
        [HttpPost]
        public async Task<IActionResult> Download([FromBody] Addic7edCreds credentials, [FromRoute] int lang, [FromRoute] int id, [FromRoute] int version, CancellationToken token)
        {
            try
            {
                var subtitleStream = await _downloader.DownloadSubtitle(credentials, lang, id, version, token);
                return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"));
            }
            catch (DownloadLimitExceededException e)
            {
                return BadRequest(e.Message);
            }
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

            if (season == null && (show.LastSeasonRefreshed == null || (DateTime.UtcNow - show.LastSeasonRefreshed) >= _timeBetweenChecks))
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

            var episode = await _episodeRepository.GetEpisodeAsync(show.Id, season.Number, request.Episode, token);
            if (episode == null && (show.LastEpisodeRefreshed == null || (DateTime.UtcNow - show.LastEpisodeRefreshed) >= _timeBetweenChecks))
            {
                var episodes = await _client.GetEpisodesAsync(request.Credentials, show, request.Season, token);
                var enumerable = episodes as Episode[] ?? episodes.ToArray();
                await _episodeRepository.UpsertEpisodes(enumerable, token);
                show.LastEpisodeRefreshed = DateTime.UtcNow;
                await _tvShowRepository.UpdateShow(show, token);
                episode = enumerable.FirstOrDefault(ep => ep.Number == request.Episode);
            }

            if (episode == null)
            {
                return NotFound(new { Error = $"Couldn't find episode S{request.Season}E{request.Episode} for {request.Show}" });
            }

            var matchingSubtitles = episode.Subtitles
                .Where(subtitle => !string.IsNullOrEmpty(subtitle.Version))
                .Where(subtitle => { return subtitle.Version.Split("+").Any(version => request.FileName.ToLowerInvariant().Contains(version.ToLowerInvariant())); })
                .ToArray();

            if (!matchingSubtitles.Any())
            {
                matchingSubtitles = episode.Subtitles.ToArray();
            }
            return Ok(new SearchResponse
            {
                Episode = new SearchResponse.EpisodeDto(episode),
                MatchingSubtitles = matchingSubtitles.Select(subtitle => new SearchResponse.SubtitleDto(subtitle))
            });
        }
    }
}