using AddictedProxy.Caching.OutputCache.Configuration;
using AddictedProxy.Model.Dto;
using AddictedProxy.Stats.Popularity.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AddictedProxy.Controllers.Rest;

[Route("stats")]
[OutputCache(PolicyName = nameof(PolicyEnum.Stats))]
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
    public ActionResult<IAsyncEnumerable<TopShowDto>> GetTopShows([FromRoute] int top, CancellationToken token)
    {
        return Ok(_showPopularityService.GetTopPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.Total)));
    }
    
    /// <summary>
    /// Return the top show by downloads
    /// </summary>
    /// <param name="top"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("downloads/{top:range(1,50)}")]
    public ActionResult<IAsyncEnumerable<TopShowDto>> GetTopDownloads([FromRoute] int top, CancellationToken token)
    {
        return Ok(_showPopularityService.GetTopDownloadPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.TotalDownloads)));
    }
}