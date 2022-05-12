using AddictedProxy.Controllers.Hub;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Services.Provider.Shows.Hub;

public class RefreshHubManager : IRefreshHubManager
{
    private readonly IHubContext<RefreshHub, IRefreshClient> _hubContext;

    public RefreshHubManager(IHubContext<RefreshHub, IRefreshClient> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Send progress in the refresh
    /// </summary>
    /// <param name="show"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SendProgressAsync(TvShow show, int progress, CancellationToken token)
    {
        return _hubContext.Clients.Group(show.UniqueId.ToString()).Progress(new ProgressDto(show.UniqueId, progress), token);
    }

    /// <summary>
    /// Send the refresh is done
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SendRefreshDone(TvShow show, CancellationToken token)
    {
        return _hubContext.Clients.Group(show.UniqueId.ToString()).Done(new ShowDto(show), token);
    }
}