using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database;

public interface ITvShowRepository
{
    IAsyncEnumerable<TvShow> FindAsync(string name);
    Task UpsertAsync(IEnumerable<TvShow> tvShows, CancellationToken token);
    IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token);
}