#region

using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

#endregion

namespace AddictedProxy.Services.Saver;

public class RefreshShowJob : IRecurringJob
{
    private readonly IServiceProvider _serviceProvider;

    public RefreshShowJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var serviceScope = _serviceProvider.CreateAsyncScope();
        var service = serviceScope.ServiceProvider.GetRequiredService<IAddictedSaver>();

        await service.RefreshShowsAsync(cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), null);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(1);
    public TimeSpan Delay { get; } = TimeSpan.FromDays(1);
}