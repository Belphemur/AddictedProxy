using System.ComponentModel.DataAnnotations;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace AddictedProxy.Controllers.Rest;

[Route("shows")]
public class TvShowsController : Controller
{
    private readonly IShowRefresher _showRefresher;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ICultureParser _cultureParser;

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

    public record ShowSearchResponse(IEnumerable<ShowDto> Shows);

    public TvShowsController(IShowRefresher showRefresher, IEpisodeRepository episodeRepository, ICultureParser cultureParser)
    {
        _showRefresher = showRefresher;
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
    }

    /// <summary>
    /// Search shows that contains the given query
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="search">Name of the show to search for</param>
    /// <response code="200">Returns the matching shows</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    [Route("search/{**search:minlength(3)}")]
    [HttpGet]
    [ProducesResponseType(typeof(ShowSearchResponse), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(typeof(string), 404)]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    public async Task<Results<Ok<ShowSearchResponse>, NotFound<string>>> Search(string search, CancellationToken cancellationToken)
    {
        var shows = await _showRefresher.FindShowsAsync(search.Trim(), cancellationToken)
                                        .Select(show => new ShowDto(show))
                                        .ToArrayAsync(cancellationToken);
        return ShowsToActionResult(search, shows);
    }

    private Results<Ok<ShowSearchResponse>, NotFound<string>> ShowsToActionResult<T>(T search, ShowDto[] shows)
    {
        if (shows.Length == 0)
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(0.5)
            };
            return TypedResults.NotFound($"Couldn't find show: {search}");
        }

        return TypedResults.Ok(new ShowSearchResponse(shows));
    }
    



    /// <summary>
    /// Get a show by it's TvDB id: https://thetvdb.com/
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="tvdbId">Id of the show on TvDB</param>
    /// <response code="200">Returns the matching shows</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    [Route("external/tvdb/{tvdbId:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(ShowSearchResponse), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(typeof(string), 404)]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    public async Task<Results<Ok<ShowSearchResponse>, NotFound<string>>> GetByTvdbId(int tvdbId, CancellationToken cancellationToken)
    {
        var shows = await _showRefresher.GetShowByTvDbIdAsync(tvdbId, cancellationToken)
                                        .Select(show => new ShowDto(show))
                                        .ToArrayAsync(cancellationToken);
        return ShowsToActionResult(tvdbId, shows);
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
    public async Task<Results<NotFound, NoContent>> Refresh([FromRoute] Guid showId, CancellationToken token)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, token);
        if (show == null)
        {
            return TypedResults.NotFound();
        }

        BackgroundJob.Enqueue<RefreshSingleShowJob>(showJob => showJob.ExecuteAsync(show.Id, default));

        return TypedResults.NoContent();
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
    public async Task<Results<Ok<TvShowSubtitleResponse>, NotFound, BadRequest<ErrorResponse>>> GetSubtitlesForSeason([FromRoute] Guid showId, [FromRoute] int seasonNumber, [FromRoute] string language, CancellationToken cancellationToken)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, cancellationToken);
        if (show == null)
        {
            return TypedResults.NotFound();
        }

        var searchLanguage = await _cultureParser.FromStringAsync(language, cancellationToken);

        if (searchLanguage == null)
        {
            return TypedResults.BadRequest(new ErrorResponse("Unknown language"));
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
        return TypedResults.Ok(new TvShowSubtitleResponse(episodes));
    }
}