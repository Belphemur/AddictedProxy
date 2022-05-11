#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class SubtitleRepository : ISubtitleRepository
{
    private readonly EntityContext _entityContext;

    public SubtitleRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
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
    /// Update the subtitle
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SaveChangeAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
}