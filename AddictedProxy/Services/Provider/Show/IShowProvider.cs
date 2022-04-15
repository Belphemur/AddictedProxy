using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Saver;

public interface IShowProvider
{
    /// <summary>
    /// Refresh the shows
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task RefreshShowsAsync(CancellationToken token);

    /// <summary>
    /// Find show having name matching <see cref="search"/>
    /// </summary>
    /// <param name="search"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token);
}