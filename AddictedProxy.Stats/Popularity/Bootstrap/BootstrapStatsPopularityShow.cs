using AddictedProxy.Stats.Popularity.Jobs;
using AddictedProxy.Stats.Popularity.Service;
using InversionOfControl.Model;
using Job.Scheduler.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Stats.Popularity.Bootstrap;

public class BootstrapStatsPopularityShow : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IShowPopularityService, ShowPopularityService>();
        services.AddJob<RecordPopularityJob>();
    }
}