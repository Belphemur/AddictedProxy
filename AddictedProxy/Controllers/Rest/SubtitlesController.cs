#region

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using AddictedProxy.Stats.Popularity.Model;
using AddictedProxy.Stats.Popularity.Service;
using AddictedProxy.Upstream.Service.Exception;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers.Rest;

[ApiController]
[Route("subtitles")]
public class SubtitlesController : Controller
{
    private readonly CultureParser _cultureParser;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly ILogger<SubtitlesController> _logger;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IShowPopularityService _showPopularityService;
    private readonly IShowRefresher _showRefresher;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly Regex _searchPattern = new(@"(?<show>.+)S(?<season>\d+)E(?<episode>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SubtitlesController(IEpisodeRepository episodeRepository,
                               CultureParser cultureParser,
                               IShowRefresher showRefresher,
                               ISubtitleProvider subtitleProvider,
                               IJobBuilder jobBuilder,
                               IJobScheduler jobScheduler,
                               ILogger<SubtitlesController> logger,
                               ISeasonRefresher seasonRefresher,
                               IEpisodeRefresher episodeRefresher,
                               IShowPopularityService showPopularityService)
    {
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
        _showRefresher = showRefresher;
        _subtitleProvider = subtitleProvider;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
        _logger = logger;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _showPopularityService = showPopularityService;
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
    [ProducesResponseType(typeof(string), 429)]
    [HttpGet]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Download([FromRoute] Guid subtitleId, CancellationToken token)
    {
        try
        {
            var subtitle = await _subtitleProvider.GetSubtitleFullAsync(subtitleId, token);
            if (subtitle == null)
            {
                return NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            var subtitleStream = await _subtitleProvider.GetSubtitleFileAsync(subtitle, token);

            var fileName =
                $"{subtitle.Episode.TvShow.Name.Replace(" ", ".")}.S{subtitle.Episode.Season:D2}E{subtitle.Episode.Number:D2}.{_cultureParser.FromString(subtitle.Language)?.TwoLetterISOLanguageName.ToLowerInvariant()}{(subtitle.HearingImpaired ? ".hi" : "")}.srt";
            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"))
            {
                EntityTag = new EntityTagHeaderValue('"' + $"{subtitle.UniqueId}{(subtitle.StoredAt.HasValue ? "-" + subtitle.StoredAt.Value.Ticks : "")}" + '"'),
                LastModified = subtitle.StoredAt,
                FileDownloadName = fileName
            };
        }
        catch (DownloadLimitExceededException e)
        {
            return StatusCode(429, e.Message);
        }
    }

    /// <summary>
    /// Search for a specific episode
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="400">Doesn't follow the right format for the search: Show S00E00</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 423)]
    [ProducesResponseType(typeof(WrongFormatResponse), 400)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken token)
    {
        var match = _searchPattern.Match(request.Search);
        if (!match.Success)
        {
            return BadRequest(new WrongFormatResponse("The search doesn't follow the wanted format. Example: Wellington S01E01", request.Search));
        }

        return await ProcessQueryRequestAsync(
            new SubtitleQueryRequest(match.Groups["show"].Value.Trim().Replace(".", " ").Replace("_", " "), int.Parse(match.Groups["episode"].Value), int.Parse(match.Groups["season"].Value), request.Language, null),
            token);
    }

    /// <summary>
    /// Query for subtitle of a specific episode of a show
    /// </summary>
    /// <remarks>
    /// The routes are ratelimited to 15 call per seconds.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    [Route("query")]
    [HttpPost]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 423)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    public async Task<IActionResult> Query([FromBody] SubtitleQueryRequest request, CancellationToken token)
    {
        return await ProcessQueryRequestAsync(request, token);
    }

    private async Task<IActionResult> ProcessQueryRequestAsync(SubtitleQueryRequest request, CancellationToken token)
    {
        _logger.LogInformation("Search for {Show} S{Season:D2}E{Episode:D2} ({Language})", request.Show, request.Season, request.Episode, request.LanguageISO);
        var show = await _showRefresher.FindShowsAsync(request.Show, token).FirstOrDefaultAsync(token);
        if (show == null)
        {
            return NotFound(new ErrorResponse($"Couldn't find the show {request.Show}"));
        }

        var language = _cultureParser.FromString(request.LanguageISO);
        if (language == null)
        {
            return BadRequest(new ErrorResponse($"Couldn't parse language {request.LanguageISO}"));
        }

        await _showPopularityService.RecordPopularityAsync(new RecordPopularityPayload(show, language), token);

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, request.Season, request.Episode, token);

        if (episode == null)
        {
            var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);
            if (season == null && !_seasonRefresher.IsShowNeedsRefresh(show))
            {
                _logger.LogInformation("Don't need to refresh seasons of show {show} returning empty data", show.Name);
                return Ok(new SubtitleSearchResponse(ArraySegment<SubtitleDto>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name)));
            }

            if (season != null && !_episodeRefresher.IsSeasonNeedRefresh(season))
            {
                _logger.LogInformation("Don't need to refresh episodes of {season} of show {show} returning empty data", request.Season, show.Name);
                return Ok(new SubtitleSearchResponse(ArraySegment<SubtitleDto>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name)));
            }

            ScheduleJob(request, show);
            return StatusCode(StatusCodes.Status423Locked, new ErrorResponse("Episode couldn't be found. Try again later."));
        }

        var matchingSubtitles = FindMatchingSubtitles(request, episode);
        if (matchingSubtitles.Length == 0)
        {
            ScheduleJob(request, show);
        }

        return Ok(new SubtitleSearchResponse(matchingSubtitles, new EpisodeDto(episode)));
    }

    private void ScheduleJob(SubtitleQueryRequest request, TvShow show)
    {
        var job = _jobBuilder.Create<FetchSubtitlesJob>()
                             .Configure(subtitlesJob => { subtitlesJob.Data = new FetchSubtitlesJob.JobData(show.Id, request.Season, request.Episode, _cultureParser.FromString(request.LanguageISO), request.FileName); })
                             .Build();
        _jobScheduler.ScheduleJob(job);
    }

    private SubtitleDto[] FindMatchingSubtitles(SubtitleQueryRequest request, Episode episode)
    {
        var searchLanguage = _cultureParser.FromString(request.LanguageISO);
        var search = episode.Subtitles
                            .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), searchLanguage));
        if (request.FileName != null)
        {
            search = search.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => request.FileName.ToLowerInvariant().Contains(version)));
        }

        return search.Select(
                         subtitle => new SubtitleDto(
                             subtitle,
                             Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                             throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                             searchLanguage
                         )
                     )
                     .ToArray();
    }

    /// <summary>
    /// Returned when the search wasn't formatted properly
    /// </summary>
    /// <param name="Error"></param>
    public record WrongFormatResponse(string Error, string Search) : ErrorResponse(Error);

    /// <summary>
    /// Use for the website to provide easy search for the user
    /// </summary>
    /// <param name="Search"></param>
    /// <param name="Language"></param>
    public record SearchRequest(string Search, string Language)
    {
        /// <summary>
        /// Search for specific subtitle
        /// </summary>
        /// <example>Wellington Paranormal S01E05</example>
        public string Search { get; init; } = Search;

        /// <summary>
        /// Language of the subtitle
        /// </summary>
        /// <example>English</example>
        public string Language { get; init; } = Language;
    }

    /// <summary>
    /// Used for different Media Center/Subtitle searchers
    /// </summary>
    public class SubtitleQueryRequest
    {
        public SubtitleQueryRequest(string show, int episode, int season, string languageIso, string? fileName)
        {
            Show = show;
            Episode = episode;
            Season = season;
            FileName = fileName;
            LanguageISO = languageIso;
        }

        /// <summary>
        /// Name of the show
        /// </summary>
        /// <example>NCIS</example>

        [Required]
        public string Show { get; }

        /// <summary>
        /// Episode number
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Episode { get; }

        /// <summary>
        /// Season number
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Season { get; }

        /// <summary>
        /// Name of the file for which you want subtitle, it help find a version of the subtitle that matches it
        /// </summary>
        /// <example>NCIS.S01E01.HDTV.mkv</example>
        /// <remarks>Optional, only if you want to match by version of subtitle</remarks>
        public string? FileName { get; }

        /// <summary>
        ///     3 or 2 letter code of the language
        /// </summary>
        /// <example>en</example>
        [Required]
        public string LanguageISO { get; }
    }
}