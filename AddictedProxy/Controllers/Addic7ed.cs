#region

using System.Globalization;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Job;
using AddictedProxy.Services.Saver;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.Exception;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers;

[ApiController]
[Route("subtitles")]
public class Addic7ed : Controller
{
    private readonly IShowProvider _showProvider;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly CultureParser _cultureParser;
    private readonly IEpisodeRepository _episodeRepository;

    public Addic7ed(IEpisodeRepository episodeRepository,
                    CultureParser cultureParser,
                    IShowProvider showProvider,
                    ISubtitleProvider subtitleProvider,
                    IJobBuilder jobBuilder,
                    IJobScheduler jobScheduler)
    {
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
        _showProvider = showProvider;
        _subtitleProvider = subtitleProvider;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
    }


    /// <summary>
    /// Download specific subtitle
    /// </summary>
    /// <param name="subtitleId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("download/{subtitleId:guid}", Name = nameof(Routes.DownloadSubtitle))]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400, "application/json")]
    [HttpGet]
    public async Task<IActionResult> Download([FromRoute] Guid subtitleId, CancellationToken token)
    {
        try
        {
            var subtitle = await _subtitleProvider.GetSubtitleAsync(subtitleId, token);
            if (subtitle == null)
            {
                return NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            var subtitleStream = await _subtitleProvider.GetSubtitleFileAsync(subtitle, token);

            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"))
            {
                EntityTag = new EntityTagHeaderValue('"' + $"{subtitle.UniqueId}-{(subtitle.StoredAt.HasValue ? "-" + subtitle.StoredAt.Value.Ticks : "")}" + '"'),
                LastModified = subtitle.StoredAt
            };
        }
        catch (DownloadLimitExceededException e)
        {
            return BadRequest(new ErrorResponse(e.Message));
        }
    }

    /// <summary>
    /// Search for subtitle of a specific episode of a show
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Produces("application/json")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken token)
    {
        var show = await _showProvider.FindShowsAsync(request.Show, token).FirstOrDefaultAsync(token);
        if (show == null)
        {
            return NotFound(new ErrorResponse($"Couldn't find the show {request.Show}"));
        }

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, request.Season, request.Episode, token);
        if (episode == null)
        {
            ScheduleJob(request, show);
            return NotFound(new ErrorResponse("Episode couldn't be found. Try again later."));
        }

        var matchingSubtitles = FindMatchingSubtitles(request, episode);
        if (matchingSubtitles.Length == 0)
        {
            ScheduleJob(request, show);
        }

        return Ok(new SearchResponse(matchingSubtitles, new SearchResponse.EpisodeDto(episode)));
    }

    private void ScheduleJob(SearchRequest request, TvShow show)
    {
        var job = _jobBuilder.Create<FetchSubtitlesJob>()
                             .Configure(subtitlesJob => { subtitlesJob.Data = new FetchSubtitlesJob.JobData(show, request.Season, request.Episode, _cultureParser.FromString(request.LanguageISO), request.FileName); })
                             .Build();
        _jobScheduler.ScheduleJob(job);
    }

    private SearchResponse.SubtitleDto[] FindMatchingSubtitles(SearchRequest request, Episode episode)
    {
        var searchLanguage = _cultureParser.FromString(request.LanguageISO);
        return episode.Subtitles
                      .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), searchLanguage))
                      .Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => request.FileName.ToLowerInvariant().Contains(version)))
                      .Select(
                          subtitle => new SearchResponse.SubtitleDto(
                              subtitle,
                              Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                              throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                              searchLanguage
                          )
                      )
                      .ToArray();
    }

    public record ErrorResponse(string Error);

    public class SearchRequest
    {
        public SearchRequest(string show, int episode, int season, string fileName, string languageIso)
        {
            Show = show;
            Episode = episode;
            Season = season;
            FileName = fileName;
            LanguageISO = languageIso;
        }

        public string Show { get; }
        public int Episode { get; }
        public int Season { get; }
        public string FileName { get; }

        /// <summary>
        ///     3 letter code of the language
        /// </summary>
        public string LanguageISO { get; }
    }

    public class SearchResponse
    {
        public SearchResponse(IEnumerable<SubtitleDto> matchingSubtitles, EpisodeDto episode)
        {
            MatchingSubtitles = matchingSubtitles;
            Episode = episode;
        }

        /// <summary>
        /// Matching subtitle for the filename and language
        /// </summary>
        public IEnumerable<SubtitleDto> MatchingSubtitles { get; }

        /// <summary>
        /// Information about the episode
        /// </summary>
        public EpisodeDto Episode { get; }

        public class SubtitleDto
        {
            public SubtitleDto(Subtitle subtitle, string downloadUri, CultureInfo? language)
            {
                Version = subtitle.Scene;
                Completed = subtitle.Completed;
                HearingImpaired = subtitle.HearingImpaired;
                HD = subtitle.HD;
                Corrected = subtitle.Completed;
                DownloadUri = downloadUri;
                Language = language?.EnglishName ?? "Unknown";
                Discovered = subtitle.Discovered;
                SubtitleId = subtitle.UniqueId.ToString();
            }

            /// <summary>
            /// Version of the subtitle
            /// </summary>
            public string Version { get; }

            public bool Completed { get; }
            public bool HearingImpaired { get; }
            public bool Corrected { get; }
            public bool HD { get; }
            public string DownloadUri { get; }

            /// <summary>
            /// Language of the subtitle (in English)
            /// </summary>
            public string Language { get; }

            /// <summary>
            ///     When was the subtitle discovered
            /// </summary>
            public DateTime Discovered { get; }

            /// <summary>
            /// Unique Id of the subtitle
            /// </summary>
            public string SubtitleId { get; }
        }

        /// <summary>
        /// Episode information
        /// </summary>
        public class EpisodeDto
        {
            public EpisodeDto(Episode episode)
            {
                Season = episode.Season;
                Number = episode.Number;
                Title = episode.Title;
                Discovered = episode.Discovered;
                Show = episode.TvShow.Name;
            }

            /// <summary>
            /// Season of the episode
            /// </summary>
            public int Season { get; }

            /// <summary>
            /// Number of the episode
            /// </summary>
            public int Number { get; }

            /// <summary>
            /// Title of the episode
            /// </summary>
            public string Title { get; }

            /// <summary>
            /// For which show
            /// </summary>
            public string Show { get; }

            /// <summary>
            ///     When was the subtitle discovered
            /// </summary>
            public DateTime Discovered { get; }
        }
    }
}