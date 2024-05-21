#region

using System.Runtime.CompilerServices;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class SubtitleRepository : ISubtitleRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;

    public SubtitleRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    /// <summary>
    ///     Get Subtitle by their Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<Subtitle?> GetSubtitleByIdAsync(int id, CancellationToken token)
    {
        return _entityContext.Subtitles.FirstOrDefaultAsync(subtitle => subtitle.Id == id, token);
    }

    /// <summary>
    ///     Get Subtitle by their Id
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="withEpisode"></param>
    /// <param name="withShow"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<Subtitle?> GetSubtitleByGuidAsync(Guid uniqueId, bool withEpisode, bool withShow, CancellationToken token = default)
    {
        var query = _entityContext.Subtitles.AsQueryable();
        if (withEpisode)
        {
            query = query.Include(subtitle => subtitle.Episode);
        }

        if (withShow)
        {
            query = query.Include(subtitle => subtitle.Episode.TvShow);
        }

        //Weird case where I can't match always the GUID. Might be an issue with SQLite
        return query.SingleOrDefaultAsync(subtitle => subtitle.UniqueId == uniqueId, token);
    }

    /// <summary>
    /// Tag subtitle for removal
    /// </summary>
    /// <param name="subtitle"></param>
    public void TagForRemoval(Subtitle subtitle)
    {
        _entityContext.Remove(subtitle);
    }
    /// <summary>
    /// Update the subtitle
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SaveChangeAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
    
    /// <summary>
    /// Increment download count
    /// </summary>
    /// <param name="subtitle"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task IncrementDownloadCountAsync(Subtitle subtitle, CancellationToken token)
    {
        return _transactionManager.WrapInTransactionAsync(async () =>
        {
            var sql = FormattableStringFactory.Create("""UPDATE "Subtitles" SET "DownloadCount" = "DownloadCount" + 1, "UpdatedAt" = now() WHERE "Id" = {0}""", subtitle.Id);

            await _entityContext.Database.ExecuteSqlAsync(sql, token);
        }, token);

    }
}