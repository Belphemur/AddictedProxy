using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.Shows;

public class ShowExternalIdRepository : IShowExternalIdRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;

    public ShowExternalIdRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    public Task<ShowExternalId?> GetBySourceAndExternalIdAsync(DataSource source, string externalId, CancellationToken token)
    {
        return _entityContext.ShowExternalIds
            .Include(e => e.TvShow)
            .ThenInclude(s => s.Seasons)
            .FirstOrDefaultAsync(e => e.Source == source && e.ExternalId == externalId, token);
    }

    public async Task<IReadOnlyList<ShowExternalId>> GetByShowIdAsync(long tvShowId, CancellationToken token)
    {
        return await _entityContext.ShowExternalIds
            .Where(e => e.TvShowId == tvShowId)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public Task<ShowExternalId?> GetByShowAndSourceAsync(long tvShowId, DataSource source, CancellationToken token)
    {
        return _entityContext.ShowExternalIds
            .FirstOrDefaultAsync(e => e.TvShowId == tvShowId && e.Source == source, token);
    }

    public async Task UpsertAsync(ShowExternalId showExternalId, CancellationToken token)
    {
        var existing = await _entityContext.ShowExternalIds
            .FirstOrDefaultAsync(e => e.TvShowId == showExternalId.TvShowId && e.Source == showExternalId.Source, token);

        if (existing != null)
        {
            existing.ExternalId = showExternalId.ExternalId;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            await _entityContext.ShowExternalIds.AddAsync(showExternalId, token);
        }

        await _entityContext.SaveChangesAsync(token);
    }

    public async Task BulkUpsertAsync(IEnumerable<ShowExternalId> showExternalIds, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var array = showExternalIds as ShowExternalId[] ?? showExternalIds.ToArray();
            if (array.Length == 0)
            {
                return;
            }

            await _entityContext.ShowExternalIds.BulkMergeAsync(array, options =>
            {
                options.ColumnPrimaryKeyExpression = e => new { e.TvShowId, e.Source };
                options.IgnoreOnMergeInsertExpression = e => e.Id;
                options.IgnoreOnMergeUpdateExpression = e => new { e.Id, e.CreatedAt };
            }, token);
        }, token);
    }
}
