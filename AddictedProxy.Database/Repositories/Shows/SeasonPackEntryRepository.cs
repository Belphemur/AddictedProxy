using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.Shows;

public class SeasonPackEntryRepository : ISeasonPackEntryRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;

    public SeasonPackEntryRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    public async Task BulkSyncAsync(long seasonPackSubtitleId, IEnumerable<SeasonPackEntry> entries, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var array = entries as SeasonPackEntry[] ?? entries.ToArray();
            if (array.Length == 0)
            {
                await _entityContext.SeasonPackEntries
                    .Where(entry => entry.SeasonPackSubtitleId == seasonPackSubtitleId)
                    .ExecuteDeleteAsync(token);
                return;
            }

            await _entityContext.SeasonPackEntries.BulkSynchronizeAsync(array, options =>
            {
                options.ColumnPrimaryKeyExpression = e => new { e.SeasonPackSubtitleId, e.FileName };
                options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SeasonPackSubtitleId;
                options.IgnoreOnSynchronizeInsertExpression = e => e.Id;
                options.IgnoreOnSynchronizeUpdateExpression = e => new { e.Id, e.CreatedAt, e.UniqueId };
            }, token);
        }, token);
    }

    public Task<bool> HasEntriesAsync(long seasonPackSubtitleId, CancellationToken token)
    {
        return _entityContext.SeasonPackEntries
            .AnyAsync(e => e.SeasonPackSubtitleId == seasonPackSubtitleId, token);
    }

    public async Task<IReadOnlyList<SeasonPackEntry>> GetBySeasonPackAsync(long seasonPackSubtitleId, CancellationToken token)
    {
        return await _entityContext.SeasonPackEntries
            .Where(e => e.SeasonPackSubtitleId == seasonPackSubtitleId)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public Task<bool> HasEpisodeAsync(long seasonPackSubtitleId, int episodeNumber, CancellationToken token)
    {
        return _entityContext.SeasonPackEntries
            .AnyAsync(e => e.SeasonPackSubtitleId == seasonPackSubtitleId && e.EpisodeNumber == episodeNumber, token);
    }

    public Task<SeasonPackEntry?> FindByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _entityContext.SeasonPackEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UniqueId == uniqueId, token);
    }

    public async Task<IReadOnlyList<SeasonPackEntry>> FindAllByEpisodeNumberAsync(long seasonPackSubtitleId, int episodeNumber, CancellationToken token)
    {
        return await _entityContext.SeasonPackEntries
            .Where(e => e.SeasonPackSubtitleId == seasonPackSubtitleId && e.EpisodeNumber == episodeNumber)
            .AsNoTracking()
            .ToListAsync(token);
    }
}
