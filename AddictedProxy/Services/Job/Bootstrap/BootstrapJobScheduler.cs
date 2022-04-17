#region

using AddictedProxy.Services.Saver;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddJob<RefreshShowJob>();
        services.AddJobScheduler(config => config.AddStartupJob(builder => builder.Create<RefreshShowJob>().Build()));
    }
}