using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Performance.Model;
using Performance.Service;

namespace Performance.Bootstrap;

public class BootstrapPerformanceSentry : IBootstrapConditional, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPerformanceTracker, PerformanceTrackerSentryUpdated>();

    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;
        return perf.Type == PerformanceConfig.BackendType.Sentry;
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseSentryTracing();
    }
}