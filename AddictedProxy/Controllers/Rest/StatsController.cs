using AddictedProxy.Model.Dto;
using AddictedProxy.Stats.Popularity.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
    /// Return the top show by downloads
    /// </summary>
    /// <param name="top"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("downloads/{top:range(1,50)}")]
    public async Task<Ok<TopShowDto[]>> GetTopDownloads([FromRoute] int top, CancellationToken token)
    {
        return TypedResults.Ok(await _showPopularityService.GetTopDownloadPopularity(top).Select(record => new TopShowDto(new ShowDto(record.Show), record.TotalDownloads)).ToArrayAsync(token));
    }
}