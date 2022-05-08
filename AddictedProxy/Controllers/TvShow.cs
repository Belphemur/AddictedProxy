using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers;

[Route("shows")]
public class TvShows : Controller
{
    private readonly IShowRefresher _showRefresher;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;

    /// <summary>
    /// Search for a specific show
    /// </summary>
    public record ShowSearchRequest(string Query)
    {
        /// <summary>
        /// Search run on the shows
        /// </summary>
        /// <example>Wellington</example>
        public string Query { get; init; } = Query;
    }

    public record ShowSearchResponse(IAsyncEnumerable<ShowDto> Shows);

    public TvShows(IShowRefresher showRefresher, IJobBuilder jobBuilder, IJobScheduler jobScheduler)
    {
        _showRefresher = showRefresher;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
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
    public async Task<IActionResult> Search([FromBody] ShowSearchRequest showSearch, CancellationToken cancellationToken)
    {
        return Ok(new ShowSearchResponse(
                _showRefresher.FindShowsAsync(showSearch.Query, cancellationToken)
                                                        .Select(show => new ShowDto(show))
            )
        );
    }

    /// <summary>
    /// Refresh a specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="404">Couldn't find the show</response>
    /// <response code="204">If the job to sync the show has been scheduled</response>
    [Route("refresh/{showId:guid}")]
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

        var job = _jobBuilder.Create<RefreshShowJob>()
                   .Configure(job => job.Show = show)
                   .Build();
        _jobScheduler.ScheduleJob(job);

        return NoContent();

    }
}