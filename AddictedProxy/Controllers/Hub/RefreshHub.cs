using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Controllers.Hub;

public class RefreshHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IShowRefresher _showRefresher;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;

    public RefreshHub(IShowRefresher showRefresher, IJobBuilder jobBuilder, IJobScheduler jobScheduler)
    {
        _showRefresher = showRefresher;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
    }

    public async Task RefreshShow(Guid showId, CancellationToken token)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, token);
        if (show == null)
        {
            return;
        }

        var job = _jobBuilder.Create<RefreshShowJob>()
                             .Configure(job => job.Show = show)
                             .Build();
        _jobScheduler.ScheduleJob(job);
        await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString(), token);
    }

    /// <summary>
    /// Send progress about refresh
    /// </summary>
    /// <param name="show"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal Task SendProgressAsync(TvShow show, int progress, CancellationToken token)
    {
        return Clients.Group(show.UniqueId.ToString()).SendAsync("progress", show.UniqueId, progress, token);
    }

    /// <summary>
    /// Send the fact the show was refreshed
    /// </summary>
    /// <param name="show"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal async Task SendRefreshDone(TvShow show, CancellationToken token)
    {
        await Clients.Group(show.UniqueId.ToString()).SendAsync("done", new ShowDto(show), token);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, show.UniqueId.ToString(), token);
    }
}