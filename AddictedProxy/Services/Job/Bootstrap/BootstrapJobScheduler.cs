#region

using InversionOfControl.Model;
using Job.Scheduler.Builder;
using Job.Scheduler.Scheduler;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IJobRunnerBuilder, JobRunnerBuilder>();
        services.AddSingleton<IJobScheduler, JobScheduler>();
        services.AddHostedService<JobSchedulerHostedService>();
    }
}