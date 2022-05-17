using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Shows.Hub;

public interface IRefreshHubManager
{
    /// <summary>
    /// Send progress in the refresh
    /// </summary>
    /// <param name="show"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SendProgressAsync(TvShow show, int progress, CancellationToken token);

    /// <summary>
    /// Send the refresh is done
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SendRefreshDone(TvShow show, CancellationToken token);
}