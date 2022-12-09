using AddictedProxy.Model.Dto;

namespace AddictedProxy.Controllers.Hub;

public interface IRefreshClient
{

    /// <summary>
    /// Send progress about refresh
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Progress(ProgressDto progress, CancellationToken token);

    /// <summary>
    /// Send the fact the show was refreshed
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Done(ShowDto show, CancellationToken token);
}