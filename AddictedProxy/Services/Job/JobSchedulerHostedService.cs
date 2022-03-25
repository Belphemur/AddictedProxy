using AddictedProxy.Services.Saver;
using Job.Scheduler.Scheduler;

namespace AddictedProxy.Services.Job;

public class JobSchedulerHostedService : IHostedService
{
    private readonly IJobScheduler _jobScheduler;
    private readonly IServiceProvider _serviceProvider;

    public JobSchedulerHostedService(IJobScheduler jobScheduler, IServiceProvider serviceProvider)
    {
        _jobScheduler = jobScheduler;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _jobScheduler.ScheduleJob(new RefreshShowJob(_serviceProvider), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _jobScheduler.StopAsync(cancellationToken);
    }
}