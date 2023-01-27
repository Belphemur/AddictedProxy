using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Sentry.Performance.Service;
using Sentry.Performance.Service.Sentry;

namespace Sentry.Performance.Bootstrap;

public class BootstrapPerformance : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPerformanceTracker, PerformanceTrackerSentry>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseEndpoints(builder =>
        {
            builder.MapMetrics().DisableRateLimiting();
        });
    }
}