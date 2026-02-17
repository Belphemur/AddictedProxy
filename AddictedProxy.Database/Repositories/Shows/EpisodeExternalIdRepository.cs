using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.Shows;

public class EpisodeExternalIdRepository : IEpisodeExternalIdRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;

    public EpisodeExternalIdRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    public Task<EpisodeExternalId?> GetBySourceAndExternalIdAsync(DataSource source, string externalId, CancellationToken token)
    {
        return _entityContext.EpisodeExternalIds
            .Include(e => e.Episode)
            .ThenInclude(ep => ep.TvShow)
            .FirstOrDefaultAsync(e => e.Source == source && e.ExternalId == externalId, token);
    }

    public async Task<IReadOnlyList<EpisodeExternalId>> GetByEpisodeIdAsync(long episodeId, CancellationToken token)
    {
        return await _entityContext.EpisodeExternalIds
            .Where(e => e.EpisodeId == episodeId)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public async Task UpsertAsync(EpisodeExternalId episodeExternalId, CancellationToken token)
    {
        var existing = await _entityContext.EpisodeExternalIds
            .FirstOrDefaultAsync(e => e.EpisodeId == episodeExternalId.EpisodeId && e.Source == episodeExternalId.Source, token);

        if (existing != null)
        {
            existing.ExternalId = episodeExternalId.ExternalId;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            await _entityContext.EpisodeExternalIds.AddAsync(episodeExternalId, token);
        }

        await _entityContext.SaveChangesAsync(token);
    }

    public async Task BulkUpsertAsync(IEnumerable<EpisodeExternalId> episodeExternalIds, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var array = episodeExternalIds as EpisodeExternalId[] ?? episodeExternalIds.ToArray();
            if (array.Length == 0)
            {
                return;
            }

            await _entityContext.EpisodeExternalIds.BulkMergeAsync(array, options =>
            {
                options.ColumnPrimaryKeyExpression = e => new { e.EpisodeId, e.Source };
                options.IgnoreOnMergeInsertExpression = e => e.Id;
                options.IgnoreOnMergeUpdateExpression = e => new { e.Id, e.CreatedAt };
            }, token);
        }, token);
    }
}
