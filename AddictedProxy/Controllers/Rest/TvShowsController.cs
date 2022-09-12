using System.ComponentModel.DataAnnotations;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers.Rest;

[Route("shows")]
public class TvShowsController : Controller
{
    private readonly IShowRefresher _showRefresher;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly CultureParser _cultureParser;

    /// <summary>
    /// Search for a specific show
    /// </summary>
    public record ShowSearchRequest([Required]
                                    [MinLength(3, ErrorMessage = "3 characters are the minimum for the search")]
                                    string Query)
    {
        /// <summary>
        /// Name of the show you're looking for
        /// </summary>
        /// <example>Wellington</example>
        public string Query { get; init; } = Query;
    }

    public record ShowSearchResponse(IAsyncEnumerable<ShowDto> Shows);

    public TvShowsController(IShowRefresher showRefresher, IJobBuilder jobBuilder, IJobScheduler jobScheduler, IEpisodeRepository episodeRepository, CultureParser cultureParser)
    {
        _showRefresher = showRefresher;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
    }

    /// <summary>
    /// Search shows that contains the given query
    /// </summary>
    /// <param name="showSearch"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the matching shows</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(ShowSearchResponse), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ShowSearchResponse), 200)]
    public Task<IActionResult> Search([FromBody] ShowSearchRequest showSearch, CancellationToken cancellationToken)
    {
        return Task.FromResult<IActionResult>(Ok(new ShowSearchResponse(
                _showRefresher.FindShowsAsync(showSearch.Query, cancellationToken)
                              .Select(show => new ShowDto(show))
            )
        ));
    }

    /// <summary>
    /// Search shows that contains the given query
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="search">Name of the show to search for</param>
    /// <response code="200">Returns the matching shows</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    [Route("search/{search}")]
    [HttpGet]
    [ProducesResponseType(typeof(ShowSearchResponse), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(typeof(string), 404)]
    [Produces("application/json")]
    [ResponseCache(Duration = 7200, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> SearchGet(string search, CancellationToken cancellationToken)
    {
        var shows = await _showRefresher.FindShowsAsync(search, cancellationToken)
                                        .Select(show => new ShowDto(show))
                                        .ToArrayAsync(cancellationToken);
        if (shows.Length == 0)
        {
            return NotFound($"Couldn't find show: {search}");
        }

        return Ok(new ShowSearchResponse(shows.ToAsyncEnumerable()));
    }


    /// <summary>
    /// Refresh a specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="404">Couldn't find the show</response>
    /// <response code="204">If the job to sync the show has been scheduled</response>
    [Route("{showId:guid}/refresh")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost]
    public async Task<IActionResult> Refresh([FromRoute] Guid showId, CancellationToken token)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, token);
        if (show == null)
        {
            return NotFound();
        }

        var job = _jobBuilder.Create<RefreshSingleShowJob>()
                             .Configure(job => job.Show = show)
                             .Build();
        _jobScheduler.ScheduleJob(job);

        return NoContent();
    }


    /// <summary>
    /// Get all subtitle of the given season for a specific language
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="seasonNumber"></param>
    /// <param name="language"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="404">Couldn't find the show</response>
    /// <response code="400">Unknown language</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <returns></returns>
    [Route("{showId:guid}/{seasonNumber:int}/{language}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(typeof(TvShowSubtitleResponse), 200)]
    public async Task<IActionResult> GetSubtitlesForSeason([FromRoute] Guid showId, [FromRoute] int seasonNumber, [FromRoute] string language, CancellationToken cancellationToken)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, cancellationToken);
        if (show == null)
        {
            return NotFound();
        }

        var searchLanguage = _cultureParser.FromString(language);

        if (searchLanguage == null)
        {
            return BadRequest(new ErrorResponse("Unknown language"));
        }


        var episodes = _episodeRepository.GetSeasonEpisodesByLangUntrackedAsync(show.Id, searchLanguage, seasonNumber)
                                         .Select(episode =>
                                         {
                                             var subs = episode
                                                        .Subtitles
                                                        .Select(
                                                            subtitle =>
                                                                new SubtitleDto(subtitle,
                                                                    Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                                                                    throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                                                                    searchLanguage)
                                                        );
                                             return new EpisodeWithSubtitlesDto(episode, subs);
                                         });
        return Ok(new TvShowSubtitleResponse(episodes));
    }
}