using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database;

public class TvShowRepository : ITvShowRepository
{
    private readonly EntityContext _entityContext;

    public TvShowRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public IAsyncEnumerable<TvShow> FindAsync(string name)
    {
        return _entityContext.TvShows.Where(show => show.Name.Contains(name)).ToAsyncEnumerable();
    }

    public Task UpsertAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        return _entityContext.TvShows.BulkMergeAsync(tvShows, token);
    }

    public IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token)
    {
        return _entityContext.TvShows.ToAsyncEnumerable();
    }
}