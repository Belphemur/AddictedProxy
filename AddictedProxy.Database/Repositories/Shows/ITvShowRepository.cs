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
    /// <returns></returns>
    IAsyncEnumerable<TvShow> FindAsync(string name);

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

    /// <summary>
    /// Get show that have a tmdbId and are in the given completed state
    /// </summary>
    /// <param name="isCompleted"></param>
    /// <returns></returns>
    public IAsyncEnumerable<TvShow> GetCompletedShows(bool isCompleted);

    /// <summary>
    /// Bulk save async the changes
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task BulkSaveChangesAsync(CancellationToken token);

    IAsyncEnumerable<TvShow> GetByTvdbIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Get duplicate show by TmdbID
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IDictionary<int, TvShow[]>> GetDuplicateTvShowByTmdbIdAsync(CancellationToken token);

    /// <summary>
    /// Get shows having at least one season
    /// </summary>
    /// <returns></returns>
    IQueryable<TvShow> GetAllHavingSubtitlesAsync();

    /// <summary>
    /// Get shows with TMDB IDs
    /// </summary>
    /// <param name="tmdbIds"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> GetShowsByTmdbIdAsync(params int[] tmdbIds);

    /// <summary>
    /// Get shows missing tvdb id but have tmdbid
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> GetShowsWithoutTvdbIdWithTmdbIdAsync();

    /// <summary>
    /// Insert a new show into the database
    /// </summary>
    /// <param name="show">The show to insert. Will have its Id populated after insert.</param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task InsertShowAsync(TvShow show, CancellationToken token);
}