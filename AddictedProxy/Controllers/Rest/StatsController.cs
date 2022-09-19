using AddictedProxy.Caching.OutputCache.Configuration;
using AddictedProxy.Model.Dto;
using AddictedProxy.Stats.Popularity.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AddictedProxy.Controllers.Rest;

[Route("stats")]
[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 6 * 3600)]
public class StatsController : Controller
{
    private readonly IShowPopularityService _showPopularityService;

    public StatsController(IShowPopularityService showPopularityService)
    {
        _showPopularityService = showPopularityService;
    }

    /// <summary>
    /// Return the top show by popularity
    /// </summary>
    /// <param name="top"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("top/{top:range(1,50)}")]
    [OutputCache(PolicyName = nameof(PolicyEnum.Stats))]
    public async Task<ActionResult<TopShowDto[]>> GetTopShows([FromRoute] int top, CancellationToken token)
    {
        return Ok(await _showPopularityService.GetTopPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.Total)).ToArrayAsync(token));
    }

    /// <summary>
    /// Return the top show by downloads
    /// </summary>
    /// <param name="top"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("downloads/{top:range(1,50)}")]
    [OutputCache(PolicyName = nameof(PolicyEnum.Stats))]
    public async Task<ActionResult<TopShowDto[]>> GetTopDownloads([FromRoute] int top, CancellationToken token)
    {
        return Ok(await _showPopularityService.GetTopDownloadPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.TotalDownloads)).ToArrayAsync(token));
    }
}