#region

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Job;
using AddictedProxy.Services.Saver;
using AddictedProxy.Upstream.Service.Exception;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers;

[ApiController]
[Route("subtitles")]
public class Subtitles : Controller
{
    private readonly CultureParser _cultureParser;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly IShowProvider _showProvider;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly Regex _searchPattern = new(@"(?<show>.+)S(?<season>\d+)E(?<episode>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Subtitles(IEpisodeRepository episodeRepository,
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
    [ProducesResponseType(typeof(string), 429)]
    [HttpGet]
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
                $"{subtitle.Episode.TvShow.Name.Replace(" ", ".")}.S{subtitle.Episode.Season:D2}E{subtitle.Episode.Number:D2}.{_cultureParser.FromString(subtitle.Language)?.TwoLetterISOLanguageName.ToLowerInvariant()}.srt";
            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"))
            {
                EntityTag = new EntityTagHeaderValue('"' + $"{subtitle.UniqueId}-{(subtitle.StoredAt.HasValue ? "-" + subtitle.StoredAt.Value.Ticks : "")}" + '"'),
                LastModified = subtitle.StoredAt,
                FileDownloadName = fileName
            };
        }
        catch (DownloadLimitExceededException e)
        {
            return BadRequest(new ErrorResponse(e.Message));
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
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
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
            new QueryRequest(match.Groups["show"].Value.Trim().Replace(".", " "), int.Parse(match.Groups["episode"].Value), int.Parse(match.Groups["season"].Value), request.Language, null),
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
    [Route("query")]
    [HttpPost]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    public async Task<IActionResult> Query([FromBody] QueryRequest request, CancellationToken token)
    {
        return await ProcessQueryRequestAsync(request, token);
    }

    private async Task<IActionResult> ProcessQueryRequestAsync(QueryRequest request, CancellationToken token)
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

    private void ScheduleJob(QueryRequest request, TvShow show)
    {
        var job = _jobBuilder.Create<FetchSubtitlesJob>()
                             .Configure(subtitlesJob => { subtitlesJob.Data = new FetchSubtitlesJob.JobData(show, request.Season, request.Episode, _cultureParser.FromString(request.LanguageISO), request.FileName); })
                             .Build();
        _jobScheduler.ScheduleJob(job);
    }

    private SearchResponse.SubtitleDto[] FindMatchingSubtitles(QueryRequest request, Episode episode)
    {
        var searchLanguage = _cultureParser.FromString(request.LanguageISO);
        var search = episode.Subtitles
                            .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), searchLanguage));
        if (request.FileName != null)
        {
            search = search.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => request.FileName.ToLowerInvariant().Contains(version)));
        }

        return search.Select(
                         subtitle => new SearchResponse.SubtitleDto(
                             subtitle,
                             Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                             throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                             searchLanguage
                         )
                     )
                     .ToArray();
    }

    /// <summary>
    /// Returns when there is an error
    /// </summary>
    /// <param name="Error"></param>
    public record ErrorResponse(string Error);

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
    public class QueryRequest
    {
        public QueryRequest(string show, int episode, int season, string languageIso, string? fileName)
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
                DownloadCount = subtitle.DownloadCount;
            }


            /// <summary>
            /// Unique Id of the subtitle
            /// </summary>
            /// <example>1086727A-EB71-4B24-A209-7CF22374574D</example>
            public string SubtitleId { get; }

            /// <summary>
            /// Version of the subtitle
            /// </summary>
            /// <example>HDTV</example>
            public string Version { get; }

            public bool Completed { get; }
            public bool HearingImpaired { get; }
            public bool Corrected { get; }
            public bool HD { get; }

            /// <summary>
            /// Url to download the subtitle
            /// </summary>
            /// <example>/download/1086727A-EB71-4B24-A209-7CF22374574D</example>
            public string DownloadUri { get; }

            /// <summary>
            /// Language of the subtitle (in English)
            /// </summary>
            /// <example>English</example>
            public string Language { get; }

            /// <summary>
            ///     When was the subtitle discovered in UTC
            /// </summary>
            /// <example>2022-04-02T05:16:45.4001274</example>
            public DateTime Discovered { get; }

            /// <summary>
            /// Number of times the subtitle was downloaded from the proxy
            /// </summary>
            /// <example>100</example>
            public long DownloadCount { get; }
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
            /// <example>1</example>
            public int Season { get; }

            /// <summary>
            /// Number of the episode
            /// </summary>
            /// <example>1</example>
            public int Number { get; }

            /// <summary>
            /// Title of the episode
            /// </summary>
            /// <example>Demon Girl</example>
            public string Title { get; }

            /// <summary>
            /// For which show
            /// </summary>
            /// <example>Wellington Paranormal</example>
            public string Show { get; }

            /// <summary>
            ///     When was the Episode discovered
            /// </summary>
            /// <example>2022-04-02T05:16:45.3996669</example>
            public DateTime Discovered { get; }
        }
    }
}