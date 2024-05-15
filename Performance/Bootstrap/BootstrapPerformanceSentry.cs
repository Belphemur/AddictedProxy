using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Performance.Model;
using Performance.Service;
using Performance.Service.OpenTelemetry;
using Performance.Service.Sentry;

namespace Performance.Bootstrap;

public class BootstrapPerformanceSentry : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPerformanceTracker, PerformanceTrackerSentry>();

    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;
        return perf.Type == PerformanceConfig.BackendType.Sentry;
    }
}