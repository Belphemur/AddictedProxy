using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Performance.Model;
using Performance.Service;
using Performance.Service.None;

namespace Performance.Bootstrap;

public class BootstrapNone : IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<IPerformanceTracker, PerformanceTrackerNone>();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;
        return perf.Type == PerformanceConfig.BackendType.None;
    }
}