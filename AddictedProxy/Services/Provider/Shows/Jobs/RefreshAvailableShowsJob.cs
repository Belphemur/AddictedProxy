#region

using System.Text.RegularExpressions;
using AddictedProxy.Services.Job.Extensions;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Scheduler;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshAvailableShowsJob : IRecurringJob
{
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<RefreshAvailableShowsJob> _logger;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;

    public RefreshAvailableShowsJob(IShowRefresher showRefresher,
                                    IPerformanceTracker performanceTracker,
                                    ILogger<RefreshAvailableShowsJob> logger,
                                    IJobBuilder jobBuilder,
                                    IJobScheduler jobScheduler)
    {
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
        _logger = logger;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-show-list");
        await _showRefresher.RefreshShowsAsync(cancellationToken);
        var mapJob = _jobBuilder.Create<MapShowTmdbJob>().Build();
        _jobScheduler.ScheduleJob(mapJob);
    }


    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Couldn't refresh shows");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(15);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}