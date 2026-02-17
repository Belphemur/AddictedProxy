using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMovieDatabaseClient.Model.Common;
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
    Task<Genre[]> GetTvGenresAsync(CancellationToken token = default);

    /// <summary>
    /// Find a show or movie by an external ID (e.g. IMDB ID).
    /// Uses TMDB's /find/{external_id} endpoint.
    /// </summary>
    /// <param name="externalId">The external ID value (e.g. "tt1234567" for IMDB)</param>
    /// <param name="externalSource">The external source (e.g. "imdb_id", "tvdb_id")</param>
    /// <param name="token"></param>
    /// <returns>Results containing matched TV shows and movies, or null on failure</returns>
    Task<FindByExternalIdResult?> FindByExternalIdAsync(string externalId, string externalSource, CancellationToken token);

    /// <summary>
    /// Get the image hosted on TMDB
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TmdbImage?> GetImageAsync(string imagePath, CancellationToken cancellationToken);

    /// <summary>
    /// Get image metadata
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TmdbImageMetadata?> GetImageMetadataAsync(string imagePath, CancellationToken cancellationToken);
}