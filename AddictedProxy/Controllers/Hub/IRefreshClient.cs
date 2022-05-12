using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;

namespace AddictedProxy.Controllers.Hub;

public interface IRefreshClient
{
    /// <summary>
    /// Trigger the refresh of a show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    public Task RefreshShow(Guid showId, CancellationToken token);

    /// <summary>
    /// Send progress about refresh
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Progress(Guid showId, int progress, CancellationToken token);

    /// <summary>
    /// Send the fact the show was refreshed
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Done(ShowDto show, CancellationToken token);
}