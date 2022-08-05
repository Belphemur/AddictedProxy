#region

using System.Runtime.CompilerServices;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class TvShowRepository : ITvShowRepository
{
    private static readonly Action<BulkOperation<TvShow>> AvoidUpdateDiscoveredField = Rule.AvoidUpdateDiscoveredField<TvShow>();
    private readonly EntityContext _entityContext;
    private readonly ILogger<TvShowRepository> _logger;

    public TvShowRepository(EntityContext entityContext, ILogger<TvShowRepository> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }


    public async IAsyncEnumerable<TvShow> FindAsync(string name, [EnumeratorCancellation] CancellationToken token)
    {
        _logger.LogInformation("Looking for show: {show}", name);
        var strictMatch = await _entityContext.TvShows
                                              .Where(show => show.Name == name)
                                              .Include(show => show.Seasons)
                                              .OrderByDescending(show => show.Priority)
                                              .FirstOrDefaultAsync(token);
        if (strictMatch != null)
        {
            yield return strictMatch;
            _logger.LogInformation("Found exact match for {show}", name);
            yield break;
        }

        foreach (var tvShow in _entityContext.TvShows
                                             .Where(show => EF.Functions.Like(show.Name, $"%{name}%"))
                                             .OrderByDescending(show => show.Priority)
                                             .Include(show => show.Seasons))
            
        {
            yield return tvShow;
        }
    }

    public Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        return _entityContext.TvShows.BulkMergeAsync(tvShows, options =>
        {
            options.IgnoreOnMergeUpdateExpression = show => new { show.Discovered, show.LastSeasonRefreshed, show.UniqueId, show.Priority, show.TmdbId, show.IsCompleted };
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