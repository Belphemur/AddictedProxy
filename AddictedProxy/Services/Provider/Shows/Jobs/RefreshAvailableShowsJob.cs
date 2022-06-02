#region

using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshAvailableShowsJob : IRecurringJob
{
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public RefreshAvailableShowsJob(IShowRefresher showRefresher, IPerformanceTracker performanceTracker)
    {
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-show-list");
        await _showRefresher.RefreshShowsAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}