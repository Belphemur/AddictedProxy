using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database.Repositories;

public interface ITvShowRepository
{
    IAsyncEnumerable<TvShow> FindAsync(string name);
    Task UpsertAsync(IEnumerable<TvShow> tvShows, CancellationToken token);
    IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token);

    /// <summary>
    /// Update data of a show
    /// </summary>
    Task UpdateShow(TvShow show, CancellationToken token);
}