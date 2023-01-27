using System.Collections.Generic;
using AddictedProxy.Stats.Popularity.Model;

namespace AddictedProxy.Stats.Popularity.Service;

public interface IShowPopularityService
{
    /// <summary>
    /// Get top shows by downloads
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    IAsyncEnumerable<DownloadPopularity> GetTopDownloadPopularity(int limit = 10);
}