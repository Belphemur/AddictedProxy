using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Controllers.Hub;

public class RefreshHub : Hub<IRefreshClient>
{
    private readonly IShowRefresher _showRefresher;

    public RefreshHub(IShowRefresher showRefresher)
    {
        _showRefresher = showRefresher;
    }

    /// <summary>
    /// Trigger the refresh of a show
    /// </summary>
    /// <param name="showId"></param>
    public async Task RefreshShow(Guid showId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString());
        var show = await _showRefresher.GetShowByGuidAsync(showId, default);
        if (show == null)
        {
            return;
        }

        BackgroundJob.Enqueue<RefreshSingleShowJob>(showJob => showJob.ExecuteAsync(show.Id, default));
    }

    /// <summary>
    /// Unsubscribe from getting refresh information about specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    public Task UnsubscribeRefreshShow(Guid showId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, showId.ToString());
    }
}