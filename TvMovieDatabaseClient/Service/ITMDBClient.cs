using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;
using TvMovieDatabaseClient.Service.Model;

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
    IAsyncEnumerable<ShowSearchResult> SearchTvAsync(string query, CancellationToken token);

    /// <summary>
    /// Search for movie
    /// </summary>
    /// <param name="query">query to send</param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<MovieSearchResult> SearchMovieAsync(string query, CancellationToken token);

    /// <summary>
    /// Get movie details by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<MovieDetails?> GetMovieDetailsByIdAsync(int showId, CancellationToken token);

    /// <summary>
    /// Get show external ids by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ExternalIds?> GetShowExternalIdsAsync(int showId, CancellationToken token);

    /// <summary>
    /// Get movie external ids by Id
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ExternalIds?> GetMovieExternalIdsAsync(int movieId, CancellationToken token);

    /// <summary>
    /// Get trending tv shows
    /// </summary>
    /// <param name="timeWindow"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<ShowSearchResult> GetTrendingTvAsync(TimeWindowEnum timeWindow = TimeWindowEnum.week, CancellationToken token = default);

    /// <summary>
    /// Get all the genre for TV
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ShowGenres?> GetTvGenresAsync(CancellationToken token = default);
}