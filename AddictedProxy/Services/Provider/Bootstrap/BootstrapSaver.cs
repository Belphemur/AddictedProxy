#region

using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Provider.Subtitle.Job;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;

#endregion

namespace AddictedProxy.Services.Saver.Bootstrap;

public class BootstrapSaver : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddJob<StoreSubtitleJob>();
        services.AddJob<FetchSubtitlesJob>();
        services.AddScoped<IShowProvider, ShowProvider>();
        services.AddScoped<ISubtitleProvider, SubtitleProvider>();
    }
}