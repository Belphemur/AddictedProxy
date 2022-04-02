using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories;

public interface ISubtitleRepository
{
    /// <summary>
    ///     Get Subtitle by their Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Subtitle?> GetSubtitleByIdAsync(int id, CancellationToken token);
}