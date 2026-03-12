using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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
            .Include(s => s.Entries)
            .Where(s => s.TvShowId == tvShowId && s.Season == season)
            .AsNoTracking()
            .ToListAsync(token);
    }

    public Task<SeasonPackSubtitle?> GetBySourceAndExternalIdAsync(DataSource source, long externalId, CancellationToken token)
    {
        return _entityContext.SeasonPackSubtitles
            .FirstOrDefaultAsync(s => s.Source == source && s.ExternalId == externalId, token);
    }

    public Task<SeasonPackSubtitle?> GetByUniqueIdAsync(Guid uniqueId, CancellationToken token)
    {
        return _entityContext.SeasonPackSubtitles
            .Include(s => s.TvShow)
            .FirstOrDefaultAsync(s => s.UniqueId == uniqueId, token);
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
                options.IgnoreOnMergeUpdateExpression = s => new { s.Id, s.Discovered, s.CreatedAt, s.UniqueId, s.StoragePath, s.StoredAt, s.DownloadCount };
            }, token);
        }, token);
    }

    public Task IncrementDownloadCountAsync(SeasonPackSubtitle seasonPackSubtitle, CancellationToken token)
    {
        return _transactionManager.WrapInTransactionAsync(async () =>
        {
            var sql = FormattableStringFactory.Create("""UPDATE "SeasonPackSubtitles" SET "DownloadCount" = "DownloadCount" + 1, "UpdatedAt" = now() WHERE "Id" = {0}""", seasonPackSubtitle.Id);
            await _entityContext.Database.ExecuteSqlAsync(sql, token);
        }, token);
    }

    public Task SaveChangeAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
}
