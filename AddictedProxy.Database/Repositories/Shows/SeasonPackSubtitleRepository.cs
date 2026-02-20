using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.Shows;

public class SeasonPackSubtitleRepository : ISeasonPackSubtitleRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;

    public SeasonPackSubtitleRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    public async Task<IReadOnlyList<SeasonPackSubtitle>> GetByShowAndSeasonAsync(long tvShowId, int season, CancellationToken token)
    {
        return await _entityContext.SeasonPackSubtitles
            .Where(s => s.TvShowId == tvShowId && s.Season == season)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public Task<SeasonPackSubtitle?> GetBySourceAndExternalIdAsync(DataSource source, long externalId, CancellationToken token)
    {
        return _entityContext.SeasonPackSubtitles
            .FirstOrDefaultAsync(s => s.Source == source && s.ExternalId == externalId, token);
    }

    public async Task BulkUpsertAsync(IEnumerable<SeasonPackSubtitle> seasonPackSubtitles, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var array = seasonPackSubtitles as SeasonPackSubtitle[] ?? seasonPackSubtitles.ToArray();
            if (array.Length == 0)
            {
                return;
            }

            await _entityContext.SeasonPackSubtitles.BulkMergeAsync(array, options =>
            {
                options.ColumnPrimaryKeyExpression = s => new { s.Source, s.ExternalId };
                options.IgnoreOnMergeInsertExpression = s => s.Id;
                options.IgnoreOnMergeUpdateExpression = s => new { s.Id, s.Discovered, s.CreatedAt, s.UniqueId, s.StoragePath, s.StoredAt };
            }, token);
        }, token);
    }
}
