using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace AddictedProxy.Database.Repositories.Shows;

public class TvShowRepository : ITvShowRepository
{
    private static readonly Action<BulkOperation<TvShow>> AvoidUpdateDiscoveredField = Rule.AvoidUpdateDiscoveredField<TvShow>();
    private readonly EntityContext _entityContext;

    public TvShowRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }


    public async IAsyncEnumerable<TvShow> FindAsync(string name, CancellationToken token)
    {
        var strictMatch = await _entityContext.TvShows.Include(show => show.Seasons)
                                              .Where(show => show.Name.ToLower() == name.ToLower())
                                              .FirstOrDefaultAsync(token);
        if (strictMatch != null)
        {
            yield return strictMatch;
            yield break;
        }

        foreach (var tvShow in _entityContext.TvShows
                                             .Include(show => show.Seasons)
                                             .Where(show => show.Name.ToLower().Contains(name.ToLower())))
        {
            yield return tvShow;
        }
    }

    public Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        return _entityContext.TvShows.BulkMergeAsync(tvShows, options =>
        {
            options.IgnoreOnMergeUpdateExpression = show => new { show.Discovered, show.LastSeasonRefreshed };
            options.ColumnPrimaryKeyExpression = show => show.ExternalId;
        }, token);
    }

    public IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token)
    {
        return _entityContext.TvShows.ToAsyncEnumerable();
    }

    public Task UpdateShow(TvShow show, CancellationToken token)
    {
        return _entityContext.TvShows.SingleUpdateAsync(show, AvoidUpdateDiscoveredField, token);
    }
}