using AddictedProxy.Stats.Popularity.Service;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Stats.Popularity.Bootstrap;

public class BootstrapStatsPopularityShow : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IShowPopularityService, ShowPopularityService>();
    }
}