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
    }

    /// <summary>
    /// Send progress about refresh
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="showId"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal Task SendProgressAsync(string connectionId, Guid showId, int progress, CancellationToken token)
    {
        return Clients.Client(connectionId).SendAsync("progressRefresh", showId, progress, token);
    }

    /// <summary>
    /// Send the fact the show was refreshed
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal Task SendRefreshDone(string connectionId, Guid showId, CancellationToken token)
    {
        return Clients.Client(connectionId).SendAsync("showRefreshed", showId, token);
    }
}