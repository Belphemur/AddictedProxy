using System.Diagnostics;
using System.Reflection;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using Prometheus;
using Performance.Service;
using Performance.Service.OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Performance.Model;

namespace Performance.Bootstrap;

public class BootstrapPerformance : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        var applicationName = entryAssembly?.GetName().Name ?? "Addicted";
        var serviceVersion = entryAssembly?.GetName().Version?.ToString() ?? "1.0.0";
        var activitySource = new ActivitySource(applicationName, serviceVersion);


        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;

        services.AddOpenTelemetry()
                .ConfigureResource(builder => builder.AddService(serviceName: applicationName, serviceVersion: serviceVersion, serviceInstanceId: Environment.MachineName))
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(activitySource.Name)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHangfireInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                            options.SetDbStatementForStoredProcedure = true;
                        })
#if !DEBUG
                        .AddOtlpExporter(options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(perf.Endpoint);
                        })
#endif
                        .SetSampler(_ => new TraceIdRatioBasedSampler(perf.SampleRate));
                });
        services.AddSingleton<IPerformanceTracker>(_ => new PerformanceTrackerOtlp(activitySource));
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseEndpoints(builder => { builder.MapMetrics().DisableRateLimiting(); });
    }
}