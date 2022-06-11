using AddictedProxy.Model.Dto;
using AddictedProxy.Stats.Popularity.Service;
using Microsoft.AspNetCore.Mvc;

namespace AddictedProxy.Controllers.Rest;

[Route("stats")]
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
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    [Route("top/{top:range(1,50)}")]
    public ActionResult<IAsyncEnumerable<TopShowDto>> GetTopShows([FromRoute] int top, CancellationToken token)
    {
        return Ok(_showPopularityService.GetTopPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.Total)));
    }
}