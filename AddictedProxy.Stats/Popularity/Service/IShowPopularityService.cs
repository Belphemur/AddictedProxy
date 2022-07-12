using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Stats.Popularity.Model;

namespace AddictedProxy.Stats.Popularity.Service;

public interface IShowPopularityService
{
    /// <summary>
    /// Record the search of a specific show
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RecordPopularityAsync(RecordPopularityPayload payload, CancellationToken cancellationToken);

    /// <summary>
    /// Get the top popular show, order by total request count
    /// </summary>
    /// <param name="limit">Default top 10</param>
    /// <returns></returns>
    IAsyncEnumerable<PopularityRecord> GetTopPopularity(int limit = 10);

    /// <summary>
    /// Get top shows by downloads
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    IAsyncEnumerable<DownloadPopularity> GetTopDownloadPopularity(int limit = 10);
}