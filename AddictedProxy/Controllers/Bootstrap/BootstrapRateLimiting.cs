using System.Globalization;
using System.Threading.RateLimiting;
using AddictedProxy.Model.RateLimiting;
using AngleSharp.Io;
using InversionOfControl.Model;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapRateLimiting : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitingConfig>(configuration.GetSection("RateLimiting"));

        services.AddRateLimiter(_ => { });
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseRateLimiter(new RateLimiterOptions
        {
            RejectionStatusCode = 429,
            GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var config = app.ApplicationServices.GetRequiredService<IOptions<RateLimitingConfig>>().Value;
                return new RateLimitPartition<string>(context.Request.Headers[config.Header].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString() ?? "default",
                    _ => new TokenBucketRateLimiter(config.Token));
            }),
            OnRejected = (context, token) =>
            {
                var lease = context.Lease;
                if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.Add(HeaderNames.RetryAfter, ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
                }

                if (lease.TryGetMetadata(MetadataName.ReasonPhrase, out var reason) && reason != null)
                {
                    context.HttpContext.Response.Headers.Add("X-Retry-Reason", reason);
                }

                return ValueTask.CompletedTask;
            }
        });
    }
}