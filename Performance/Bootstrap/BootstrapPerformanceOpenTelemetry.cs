using System.Diagnostics;
using System.Reflection;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
#if !DEBUG
using OpenTelemetry.Exporter;
#endif
using OpenTelemetry.Resources;
using Prometheus;
using Performance.Service;
using Performance.Service.OpenTelemetry;
using OpenTelemetry.Trace;
using Performance.Model;
using Performance.Service.OpenTelemetry.Processor;
using Performance.Service.OpenTelemetry.Sampler;

namespace Performance.Bootstrap;

public class BootstrapPerformanceOpenTelemetry : IBootstrapApp, IBootstrapConditional
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        var applicationName = entryAssembly?.GetName().Name ?? "Addicted";
        var serviceVersion = entryAssembly?.GetName().Version?.ToString() ?? "1.0.0";
        var activitySource = new ActivitySource(applicationName, serviceVersion);


        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;

        var telemetryBuilder = services.AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(serviceName: applicationName, serviceVersion: serviceVersion, serviceInstanceId: Environment.MachineName))
            .WithTracing(builder =>
            {
                builder
                    .AddSource(activitySource.Name)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddAWSInstrumentation()
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
#else
                    .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug)
#endif
                    .SetSampler(new AlwaysRecordSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(perf.SampleRate))))
                    .AddProcessor<TailRecordErrorProcessor>();
            });
        if (perf.SendLogs)
        {
            telemetryBuilder.WithLogging(builder =>
            {
                builder
                    .AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = new Uri(perf.Endpoint);
                    });
#if DEBUG
                builder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);
#endif
            });
        }

        services.AddSingleton<IPerformanceTracker>(_ => new PerformanceTrackerOtlp(activitySource));
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        var perf = configuration.GetSection("Performance").Get<PerformanceConfig>()!;
        return perf.Type == PerformanceConfig.BackendType.OpenTelemetry;
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseEndpoints(builder => { builder.MapMetrics().DisableRateLimiting(); });
    }
}