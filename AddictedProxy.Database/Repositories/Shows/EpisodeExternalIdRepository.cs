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
        var now = DateTime.UtcNow;
        // Atomic upsert via CTE:
        // 1. Try to UPDATE the existing row for (EpisodeId, Source) — sets the new ExternalId
        // 2. If no row was updated (no existing mapping), INSERT a new row
        // 3. If the INSERT conflicts on (Source, ExternalId) — same external ID already
        //    mapped to a different episode — ignore it gracefully (DO NOTHING)
        await _entityContext.Database.ExecuteSqlAsync(
            $"""
             WITH update_by_episode AS (
                 UPDATE "EpisodeExternalIds"
                 SET "ExternalId" = {episodeExternalId.ExternalId}, "UpdatedAt" = {now}
                 WHERE "EpisodeId" = {episodeExternalId.EpisodeId} AND "Source" = {(int)episodeExternalId.Source}
                 RETURNING "Id"
             )
             INSERT INTO "EpisodeExternalIds" ("EpisodeId", "Source", "ExternalId", "CreatedAt", "UpdatedAt")
             SELECT {episodeExternalId.EpisodeId}, {(int)episodeExternalId.Source}, {episodeExternalId.ExternalId}, {now}, {now}
             WHERE NOT EXISTS (SELECT 1 FROM update_by_episode)
             ON CONFLICT ("Source", "ExternalId") DO NOTHING
             """,
            token);
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
