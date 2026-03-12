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

    public async Task BulkUpsertAsync(IEnumerable<SeasonPackEntry> entries, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var array = entries as SeasonPackEntry[] ?? entries.ToArray();
            if (array.Length == 0)
            {
                return;
            }

            await _entityContext.SeasonPackEntries.BulkMergeAsync(array, options =>
            {
                options.ColumnPrimaryKeyExpression = e => new { e.SeasonPackSubtitleId, e.FileName };
                options.IgnoreOnMergeInsertExpression = e => e.Id;
                options.IgnoreOnMergeUpdateExpression = e => new { e.Id, e.CreatedAt };
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
}
