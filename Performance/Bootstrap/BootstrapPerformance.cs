using System.Diagnostics;
using System.Reflection;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using Prometheus;
using Sentry.Performance.Service;
using Sentry.Performance.Service.OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentry.Performance.Model;

namespace Sentry.Performance.Bootstrap;

public class BootstrapPerformance : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        var entryAssemblyFullName = entryAssembly.FullName ?? "Addicted";
        var activitySource = new ActivitySource(entryAssemblyFullName);


        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;

        services.AddOpenTelemetry()
                .ConfigureResource(builder => builder.AddService(serviceName: entryAssemblyFullName, serviceVersion: entryAssembly.GetName().Version?.ToString() ?? "1.0.0", serviceInstanceId: Environment.MachineName))
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(activitySource.Name)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddRedisInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(perf.Endpoint);
                        })
#if DEBUG
                        .AddConsoleExporter()
#endif
                        .SetSampler(_ => new TraceIdRatioBasedSampler(perf.SampleRate));
                });
        services.AddScoped<IPerformanceTracker>(provider => new PerformanceTrackerOtlp(activitySource));
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseEndpoints(builder => { builder.MapMetrics().DisableRateLimiting(); });
    }
}