using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Controllers.Hub;

public class RefreshHub : Hub<IRefreshClient>
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

        var job = _jobBuilder.Create<RefreshSignleShowJob>()
                             .Configure(job => job.Show = show)
                             .Build();
        _jobScheduler.ScheduleJob(job);
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