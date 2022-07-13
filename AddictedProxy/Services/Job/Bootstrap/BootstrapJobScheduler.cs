#region

using AddictedProxy.Services.Credentials.Job;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;
using Job.Scheduler.Queue;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddJob<RefreshAvailableShowsJob>();
        services.AddJobScheduler(
            config => config
                      .AddStartupJob(builder => builder.Create<RefreshAvailableShowsJob>().Build())
                      .AddStartupJob(builder => builder.Create<DownloadCredsRedeemerJob>().Build())
                      .RegisterQueue(new QueueSettings(nameof(FetchSubtitlesJob), 15))
        );
    }
}