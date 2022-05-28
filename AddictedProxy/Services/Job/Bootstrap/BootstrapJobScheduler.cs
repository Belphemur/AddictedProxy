#region

using AddictedProxy.Services.Credentials.Job;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddJob<RefreshShowsJob>();
        services.AddJobScheduler(
            config => config
                      .AddStartupJob(builder => builder.Create<RefreshShowsJob>().Build())
                      .AddStartupJob(builder => builder.Create<DownloadCredsRedeemerJob>().Build())
        );
    }
}