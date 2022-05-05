#region

using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

#endregion

namespace AddictedProxy.Services.Provider.Show;

public class RefreshShowJob : IRecurringJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IShowProvider _showProvider;

    public RefreshShowJob(IShowProvider showProvider)
    {
        _showProvider = showProvider;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _showProvider.RefreshShowsAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}