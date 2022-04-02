using AddictedProxy.Database.Context;
using AddictedProxy.Model.Shows;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories;

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
}