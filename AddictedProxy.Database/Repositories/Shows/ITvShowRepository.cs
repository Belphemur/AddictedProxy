using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface ITvShowRepository
{
    IAsyncEnumerable<TvShow> FindAsync(string name, CancellationToken token);
    Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token);
    IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token);

    /// <summary>
    ///     Update data of a show
    /// </summary>
    Task UpdateShow(TvShow show, CancellationToken token);
}