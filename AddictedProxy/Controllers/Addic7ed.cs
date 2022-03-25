using System.Globalization;
using AddictedProxy.Database;
using AddictedProxy.Database.Repositories;
using AddictedProxy.Model;
using AddictedProxy.Model.Config;
using AddictedProxy.Model.Shows;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Addic7ed.Exception;
using AddictedProxy.Services.Culture;
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
        private readonly ISubtitleRepository _subtitleRepository;
        private readonly CultureParser _cultureParser;
        private readonly TimeSpan _timeBetweenChecks;

        public class SearchRequest
        {
            public Addic7edCreds Credentials { get; }
            public string Show { get; }
            public int Episode { get; }
            public int Season { get; }
            public string FileName { get; }
            /// <summary>
            /// 3 letter code of the language
            /// </summary>
            public string LanguageISO { get; }

            public SearchRequest(Addic7edCreds credentials, string show, int episode, int season, string fileName, string languageIso)
            {
                Credentials = credentials;
                Show = show;
                Episode = episode;
                Season = season;
                FileName = fileName;
                LanguageISO = languageIso;
            }
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

                public SubtitleDto(Subtitle subtitle, CultureInfo? language)
                {
                    Version = subtitle.Scene;
                    Completed = subtitle.Completed;
                    HearingImpaired = subtitle.HearingImpaired;
                    HD = subtitle.HD;
                    Corrected = subtitle.Completed;
                    DownloadUri = new Uri($"/download/{subtitle.Id}");
                    Language = language?.EnglishName ?? "Unknown";
                    Discovered = subtitle.Discovered;
                }
            }

            public class EpisodeDto
            {
                public int Season { get; }
                public int Number { get; }
                public string Title { get; }
                public string Show { get; }

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
                    Show = episode.TvShow.Name;
                }
            }

            public SearchResponse(IEnumerable<SubtitleDto> matchingSubtitles, EpisodeDto episode)
            {
                MatchingSubtitles = matchingSubtitles;
                Episode = episode;
            }

            public IEnumerable<SubtitleDto> MatchingSubtitles { get; }

            public EpisodeDto Episode { get; }
        }

        public Addic7ed(IAddic7edClient client, 
            IAddic7edDownloader downloader, 
            ITvShowRepository tvShowRepository, 
            ISeasonRepository seasonRepository, 
            IEpisodeRepository episodeRepository,
            ISubtitleRepository subtitleRepository,
            CultureParser cultureParser)
        {
            _client = client;
            _downloader = downloader;
            _tvShowRepository = tvShowRepository;
            _seasonRepository = seasonRepository;
            _episodeRepository = episodeRepository;
            _subtitleRepository = subtitleRepository;
            _cultureParser = cultureParser;
            _timeBetweenChecks = TimeSpan.FromHours(1);
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

        [Route("download/{subtitleId:int}")]
        [HttpPost]
        public async Task<IActionResult> Download([FromBody] Addic7edCreds credentials, [FromRoute] int subtitleId, CancellationToken token)
        {
            var subtitle = await _subtitleRepository.GetSubtitleByIdAsync(subtitleId, token);
            if (subtitle == null)
            {
                return NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            try
            {
                var subtitleStream = await _downloader.DownloadSubtitle(credentials, subtitle, token);
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
            var show = await _tvShowRepository.FindAsync(request.Show, token).FirstOrDefaultAsync(token);
            if (show == null)
            {
                return NotFound(new { Error = $"Couldn't find the show {request.Show}" });
            }

            var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);


            if (season == null && (show.LastSeasonRefreshed == null || (DateTime.UtcNow - show.LastSeasonRefreshed) >= _timeBetweenChecks))
            {
                var maxSeason = show.Seasons.Any() ? show.Seasons.Max(s => s.Number) : 0;
                if (show.Seasons.Any() && request.Season - maxSeason > 1)
                {
                    return NotFound(new { Error = $"{request.Season} is too far in the future." });
                }

                var seasons = (await _client.GetSeasonsAsync(request.Credentials, show, token)).ToArray();
                await _seasonRepository.UpsertSeason(seasons, token);
                show.LastSeasonRefreshed = DateTime.UtcNow;
                await _tvShowRepository.UpdateShow(show, token);
                season = await _seasonRepository.GetSeasonForShow(show.Id, request.Season, token);
            }

            if (season == null)
            {
                return NotFound(new { Error = $"Couldn't find Season S{request.Season} for {show.Name}" });
            }

            var episode = await _episodeRepository.GetEpisodeAsync(show.Id, season.Number, request.Episode, token);

            var episodesRefreshed = season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= _timeBetweenChecks;
            if (episode == null && !episodesRefreshed)
            {
                episode = await RefreshSubtitlesAsync(request, show, season, token);
                episodesRefreshed = true;
            }

            if (episode == null)
            {
                return NotFound(new { Error = $"Couldn't find episode S{season.Number}E{request.Episode} for {show.Name}" });
            }

            var matchingSubtitles = FindMatchingSubtitles(request, episode);

            var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

            if (matchingSubtitles.Any() || episodesRefreshed || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
            {
                return Ok(new SearchResponse(episode: new SearchResponse.EpisodeDto(episode), matchingSubtitles: matchingSubtitles));
            }

            episode = await RefreshSubtitlesAsync(request, show, season, token);
            matchingSubtitles = FindMatchingSubtitles(request, episode!);


            return Ok(new SearchResponse(episode: new SearchResponse.EpisodeDto(episode!), matchingSubtitles: matchingSubtitles));
        }

        private  SearchResponse.SubtitleDto[] FindMatchingSubtitles(SearchRequest request, Episode episode)
        {
            var searchLanguage = _cultureParser.FromString(request.LanguageISO); 
            return episode.Subtitles
                .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), searchLanguage))
                .Where(subtitle => { return subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => request.FileName.ToLowerInvariant().Contains(version)); })
                .Select(subtitle => new SearchResponse.SubtitleDto(subtitle, searchLanguage))
                .ToArray();
        }

        private async Task<Episode?> RefreshSubtitlesAsync(SearchRequest request, TvShow show, Season season, CancellationToken token)
        {
            var episodes = await _client.GetEpisodesAsync(request.Credentials, show, request.Season, token);
            await _episodeRepository.UpsertEpisodes(episodes, token);
            season.LastRefreshed = DateTime.UtcNow;
            await _seasonRepository.UpdateSeasonAsync(season, token);
            return await _episodeRepository.GetEpisodeAsync(show.Id, request.Season, request.Episode, token);
        }
    }
}