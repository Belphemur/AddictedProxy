#region

using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

#endregion

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshShowJob : IRecurringJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IShowRefresher _showRefresher;

    public RefreshShowJob(IShowRefresher showRefresher)
    {
        _showRefresher = showRefresher;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
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