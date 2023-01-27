using System.Collections.Generic;
using System.Linq;
using AddictedProxy.Database.Context;
using AddictedProxy.Stats.Popularity.Model;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Stats.Popularity.Service;

internal class ShowPopularityService : IShowPopularityService
{
    private readonly EntityContext _entityContext;

    public ShowPopularityService(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    /// <summary>
    /// Get top shows by downloads
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public IAsyncEnumerable<DownloadPopularity> GetTopDownloadPopularity(int limit = 10)
    {
        return _entityContext.Subtitles
            .Include(subtitle => subtitle.Episode.TvShow.Seasons)
            .GroupBy(subtitle => subtitle.Episode.TvShowId)
            .Select(grouping => new
            {
                grouping.First().Episode.TvShow,
                TotalDownloads = grouping.Sum(subtitle => subtitle.DownloadCount)
            })
            .OrderByDescending(arg => arg.TotalDownloads)
            .AsNoTracking()
            .Take(limit)
            .ToAsyncEnumerable()
            .Select(arg => new DownloadPopularity(arg.TvShow, arg.TotalDownloads));
    }
}