using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Stats;
using AddictedProxy.Stats.Popularity.Model;
using Locking;
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
    /// Record the search of a specific show
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RecordPopularityAsync(RecordPopularityPayload payload, CancellationToken cancellationToken)
    {
        using var namedLock = Lock<ShowPopularityService>.GetNamedLock(payload.TvShowId.ToString());

        if (!await namedLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
        {
            return;
        }

        var popularity = await _entityContext.ShowPopularity.FindAsync(new object?[] { payload.TvShowId, payload.Language.Name }, cancellationToken: cancellationToken);
        if (popularity == null)
        {
            popularity = new ShowPopularity
            {
                TvShowId = payload.TvShowId,
                Language = payload.Language.Name,
                RequestedCount = 0
            };
            await _entityContext.ShowPopularity.AddAsync(popularity, cancellationToken);
        }

        popularity.RequestedCount++;
        popularity.LastRequestedDate = payload.Requested ?? DateTime.UtcNow;

        await _entityContext.SaveChangesAsync(cancellationToken);
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

    /// <summary>
    /// Get the top popular show, order by total request count
    /// </summary>
    /// <param name="limit">Default top 10</param>
    /// <returns></returns>
    public IAsyncEnumerable<PopularityRecord> GetTopPopularity(int limit = 10)
    {
        return _entityContext.ShowPopularity
            .Include(popularity => popularity.TvShow)
            .Include(popularity => popularity.TvShow.Seasons)
            .GroupBy(popularity => popularity.TvShowId)
            .Select(grouping => new
            {
                grouping.First().TvShow,
                Group = grouping.ToArray(),
                Sum = grouping.Sum(popularity => popularity.RequestedCount)
            })
            .OrderByDescending(group => group.Sum)
            .AsNoTracking()
            .Take(limit)
            .ToAsyncEnumerable()
            .Select(grouping =>
            {
                var popularityCounts = grouping.Group.Select(popularity => (popularity.Language, new PopularityCount(popularity.RequestedCount, popularity.LastRequestedDate!.Value)))
                    .ToImmutableDictionary(tuple => CultureInfo.GetCultureInfo(tuple.Language), tuple => tuple.Item2);
                return new PopularityRecord(grouping.TvShow, popularityCounts, grouping.Sum);
            });
    }
}