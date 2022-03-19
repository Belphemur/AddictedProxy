using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace AddictedProxy.Services.Saver;

public class RefreshShowJob : IRecurringJob
{
    private readonly IAddictedSaver _addictedSaver;

    public RefreshShowJob(IAddictedSaver addictedSaver)
    {
        _addictedSaver = addictedSaver;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _addictedSaver.RefreshShows(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}