using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;
using AddictedProxy.Model.RateLimiting;
using AngleSharp.Io;
using InversionOfControl.Model;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Prometheus;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapRateLimiting : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<RateLimitingConfig>(configuration.GetSection("RateLimiting"));
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        var rateLimitCounter = Metrics.CreateCounter("http_request_rate_limited", "Number of requests that got rate limited", new[] { "controller", "action" }, new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars()
        });

        var config = app.ApplicationServices.GetRequiredService<IOptions<RateLimitingConfig>>();

        if (config.Value.Enabled)
        {
            app.UseRateLimiter(new RateLimiterOptions
            {
                RejectionStatusCode = 429,
                GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                {
                    var config = app.ApplicationServices.GetRequiredService<IOptions<RateLimitingConfig>>().Value;
                    return RateLimitPartition.GetTokenBucketLimiter(context.Connection.RemoteIpAddress, _ => config.Token)!;
                }),
                OnRejected = (context, _) =>
                {
                    var routeValues = context.HttpContext.GetRouteData().Values;
                    var controller = routeValues["controller"]?.ToString() ?? "";
                    var action = routeValues["action"]?.ToString() ?? "";
                    rateLimitCounter.Labels(controller, action).Inc();

                    var logger = app.ApplicationServices.GetRequiredService<ILogger<TokenBucketRateLimiter>>();
                    var lease = context.Lease;
                    if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.Add(HeaderNames.RetryAfter, ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
                    }

                    if (lease.TryGetMetadata(MetadataName.ReasonPhrase, out var reason) && reason != null)
                    {
                        context.HttpContext.Response.Headers.Add("X-Retry-Reason", reason);
                    }

                    logger.LogWarning("RateLimiting reached for {IP}: {retryAfter}", context.HttpContext.Connection.RemoteIpAddress, retryAfter);

                    return ValueTask.CompletedTask;
                }
            });
        }
    }
}