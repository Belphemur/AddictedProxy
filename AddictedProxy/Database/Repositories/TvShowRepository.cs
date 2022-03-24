using AddictedProxy.Database.Context;
using AddictedProxy.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace AddictedProxy.Database.Repositories;

public class TvShowRepository : ITvShowRepository
{
    private readonly EntityContext _entityContext;
    private static readonly Action<BulkOperation<TvShow>> AvoidUpdateDiscoveredField = Rule.AvoidUpdateDiscoveredField<TvShow>();

    public TvShowRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }


    public IAsyncEnumerable<TvShow> FindAsync(string name)
    {
        return _entityContext.TvShows
            .Include(show => show.Seasons)
            .Where(show => show.Name.Contains(name))
            .ToAsyncEnumerable();
    }

    public Task UpsertAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        return _entityContext.TvShows.BulkMergeAsync(tvShows, AvoidUpdateDiscoveredField, token);
    }

    public IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token)
    {
        return _entityContext.TvShows.ToAsyncEnumerable();
    }

    public Task UpdateShow(TvShow show, CancellationToken token) => _entityContext.TvShows.SingleUpdateAsync(show, AvoidUpdateDiscoveredField, token);
}