﻿#region

using System.Linq.Expressions;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Performance.Service;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class TvShowRepository : ITvShowRepository
{
    private static readonly Action<BulkOperation<TvShow>> AvoidUpdateDiscoveredField = Rule.AvoidUpdateDiscoveredField<TvShow>();
    private readonly EntityContext _entityContext;
    private readonly ILogger<TvShowRepository> _logger;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly IPerformanceTracker _performanceTracker;

    public TvShowRepository(EntityContext entityContext, 
        ILogger<TvShowRepository> logger,
        ITransactionManager<EntityContext> transactionManager,
        IPerformanceTracker performanceTracker)
    {
        _entityContext = entityContext;
        _logger = logger;
        _transactionManager = transactionManager;
        _performanceTracker = performanceTracker;
    }


    public IAsyncEnumerable<TvShow> FindAsync(string name)
    {
        _logger.LogInformation("Looking for show: {show}", name);

        return _entityContext.TvShows
                             .Where(show => EF.Functions.ILike(show.Name, $"%{name}%"))
                             .OrderByDescending(show => show.Priority)
                             .Include(show => show.Seasons)
                             .ToAsyncEnumerable();
    }

    public async Task UpsertRefreshedShowsAsync(IEnumerable<TvShow> tvShows, CancellationToken token)
    {
        using var span = _performanceTracker.BeginNestedSpan("UpsertRefreshedShowsAsync");
        Expression<Func<TvShow, object>> ignoreOnUpdate = show => new { show.Id, show.Discovered, show.CreatedAt, show.LastSeasonRefreshed, show.UniqueId, show.Priority, show.TmdbId, show.IsCompleted, show.Type, show.TvdbId, show.Source };

        var showsArray = tvShows as TvShow[] ?? tvShows.ToArray();
        span.SetTag("shows.total", showsArray.Length);
        _logger.LogInformation("Upserting {Count} shows", showsArray.Length);
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await _entityContext.TvShows.BulkMergeAsync(showsArray, options =>
            {
                options.IgnoreOnMergeUpdateExpression = ignoreOnUpdate;
                options.IgnoreOnSynchronizeUpdateExpression = ignoreOnUpdate;
                options.ColumnPrimaryKeyExpression = show => show.ExternalId;
            }, token);
        }, token);
    
    }
    public Task BulkSaveChangesAsync(CancellationToken token)
    {
        return _entityContext.BulkSaveChangesAsync(token);
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
                             .AsNoTracking()
                             .SingleOrDefaultAsync(show => show.UniqueId == id, cancellationToken: cancellationToken);
    }

    public IAsyncEnumerable<TvShow> GetByTvdbIdAsync(int id, CancellationToken cancellationToken)
    {
        return _entityContext.TvShows
                             .Include(show => show.Seasons)
                             .Where(show => show.TvdbId == id)
                             .AsNoTracking()
                             .ToAsyncEnumerable();
    }


    /// <summary>
    /// Get shows with TMDB IDs
    /// </summary>
    /// <param name="tmdbIds"></param>
    /// <returns></returns>
    public IAsyncEnumerable<TvShow> GetShowsByTmdbIdAsync(params int[] tmdbIds)
    {
        return _entityContext.TvShows
                             .Where(show => show.TmdbId.HasValue)
                             .Where(show => tmdbIds.Contains(show.TmdbId!.Value))
                             .Include(show => show.Seasons)
                             .AsNoTracking()
                             .ToAsyncEnumerable();
    }

    /// <summary>
    /// Get duplicate show by TmdbID
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IDictionary<int, TvShow[]>> GetDuplicateTvShowByTmdbIdAsync(CancellationToken token)
    {
        return await _entityContext.TvShows
                                   .GroupBy(show => show.TmdbId)
                                   .Where(shows => shows.Count() > 1)
                                   .Select(shows => new
                                   {
                                       TmdbId = shows.Key,
                                       Shows = shows.ToArray()
                                   })
                                   .Where(arg => arg.TmdbId != null)
                                   .ToDictionaryAsync(shows => shows.TmdbId!.Value, shows => shows.Shows, cancellationToken: token);
    }

    public IAsyncEnumerable<TvShow> GetShowWithoutTmdbIdAsync()
    {
        return _entityContext.TvShows
                             .Where(show => !show.TmdbId.HasValue)
                             .Include(show => show.Seasons)
                             .ToAsyncEnumerable();
    }
    public IAsyncEnumerable<TvShow> GetShowsWithoutTvdbIdWithTmdbIdAsync()
    {
        return _entityContext.TvShows
            .Where(show => !show.TvdbId.HasValue)
            .Where(show => show.TmdbId.HasValue)
            .Include(show => show.Seasons)
            .ToAsyncEnumerable();
    }

    /// <summary>
    /// Get shows having at least one season
    /// </summary>
    /// <returns></returns>
    public IQueryable<TvShow> GetAllHavingSubtitlesAsync()
    {
        return _entityContext.TvShows.Where(show => show.Episodes.First().Subtitles.Count > 0)
            .OrderBy(show => show.Id)
            .AsNoTracking();
    }

    public IAsyncEnumerable<TvShow> GetCompletedShows(bool isCompleted = false)
    {
        return _entityContext.TvShows
            .Where(show => show.TmdbId.HasValue)
            .Where(show => show.IsCompleted == isCompleted)
            .Where(show => show.Type == ShowType.Show)
            .ToAsyncEnumerable();
    }
}