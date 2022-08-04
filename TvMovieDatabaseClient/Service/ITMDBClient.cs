using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMovieDatabaseClient.Model.Search;
using TvMovieDatabaseClient.Model.Show;

namespace TvMovieDatabaseClient.Service;

public interface ITMDBClient
{
    /// <summary>
    /// Get show details by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ShowDetails?> GetShowDetailsByIdAsync(int showId, CancellationToken token);

    /// <summary>
    /// Search for tv shows
    /// </summary>
    /// <param name="query">query to send</param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerator<ShowSearchResult> SearchTvAsync(string query, CancellationToken token);
}