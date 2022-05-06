#region

using AddictedProxy.Database.Model.Shows;

#endregion

namespace AddictedProxy.Services.Provider.Shows;

public interface IShowRefresher
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

    /// <summary>
    /// Get the show by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TvShow?> GetShowByIdAsync(long id, CancellationToken cancellationToken);
}