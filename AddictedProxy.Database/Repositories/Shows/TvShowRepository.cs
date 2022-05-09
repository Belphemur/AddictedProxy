#region

using System.Runtime.CompilerServices;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class TvShowRepository : ITvShowRepository
{
    private static readonly Action<BulkOperation<TvShow>> AvoidUpdateDiscoveredField = Rule.AvoidUpdateDiscoveredField<TvShow>();
    private readonly EntityContext _entityContext;

    public TvShowRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }


    public async IAsyncEnumerable<TvShow> FindAsync(string name, [EnumeratorCancellation] CancellationToken token)
    {
        var strictMatch = await _entityContext.TvShows
                                              .Where(show => show.Name == name)
                                              .Include(show => show.Seasons)
                                              .FirstOrDefaultAsync(token);
        if (strictMatch != null)
        {
            yield return strictMatch;
            yield break;
        }

        foreach (var tvShow in _entityContext.TvShows
                                             .Where(show => EF.Functions.Like(show.Name, $"%{name}%"))
                                             .Include(show => show.Seasons))
            
        {
            yield return tvShow;
        }
    }

    public Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        return _entityContext.TvShows.BulkMergeAsync(tvShows, options =>
        {
            options.IgnoreOnMergeUpdateExpression = show => new { show.Discovered, show.LastSeasonRefreshed, show.UniqueId };
            options.ColumnPrimaryKeyExpression = show => show.ExternalId;
        }, token);
    }

    public IAsyncEnumerable<TvShow> GetAllAsync(CancellationToken token)
    {
        return _entityContext.TvShows.ToAsyncEnumerable();
    }

    public Task UpdateShowAsync(TvShow show, CancellationToken token)
    {
        return _entityContext.TvShows.SingleUpdateAsync(show, AvoidUpdateDiscoveredField, token);
    }

    public Task<TvShow?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return _entityContext.TvShows
                             .Include(show => show.Seasons)
                             .SingleOrDefaultAsync(show => show.Id == id, cancellationToken: cancellationToken);
    }

    public Task<TvShow?> GetByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        return _entityContext.TvShows
                             .Include(show => show.Seasons)
                             .SingleOrDefaultAsync(show => show.UniqueId == id, cancellationToken: cancellationToken);    }
}