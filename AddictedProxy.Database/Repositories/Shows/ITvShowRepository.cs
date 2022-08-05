#region

using AddictedProxy.Database.Model.Shows;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public interface ITvShowRepository
{
    /// <summary>
    /// Search in the database for the show with it's name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> FindAsync(string name, CancellationToken token);
    /// <summary>
    /// Upsert the information about the shows
    /// </summary>
    /// <param name="tvShows"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token);
    /// <summary>
    /// Get all the shows
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token);

    /// <summary>
    ///     Update data of a show
    /// </summary>
    Task UpdateShowAsync(TvShow show, CancellationToken token);

    /// <summary>
    /// Get Show by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TvShow?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Get show by their unique id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TvShow?> GetByGuidAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Get shows that don't have a tmdbId
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<TvShow> GetShowWithoutTmdbIdAsync();
}