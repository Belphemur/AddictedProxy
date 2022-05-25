using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry.Performance.Service;
using Sentry.Performance.Service.Sentry;

namespace Sentry.Performance.Bootstrap;

public class BootstrapPerformance : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPerformanceTracker, PerformanceTrackerSentry>();
    }
}