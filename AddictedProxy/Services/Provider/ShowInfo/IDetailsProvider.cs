using AddictedProxy.Database.Model.Shows;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;

namespace AddictedProxy.Services.Provider.ShowInfo;

public interface IDetailsProvider
{
    /// <summary>
    /// Get show info
    /// </summary>
    /// <param name="show"></param>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(ShowDetails Details, ExternalIds? ExternalIds)> GetShowInfoAsync(TvShow show, Func<ShowSearchResult, bool> filter, CancellationToken cancellationToken);

    /// <summary>
    /// Get movie info
    /// </summary>
    /// <param name="show"></param>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<(MovieDetails Details, ExternalIds? ExternalIds)> GetMovieInfoAsync(TvShow show, Func<MovieSearchResult, bool> filter, CancellationToken cancellationToken);
}