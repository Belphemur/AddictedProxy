#region

using AddictedProxy.Services.Provider.Show;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddJob<RefreshShowJob>();
        services.AddJobScheduler(config => config.AddStartupJob(builder => builder.Create<RefreshShowJob>().Build()));
    }
}