using System.Threading.RateLimiting;
using AspNetCoreRateLimit;
using InversionOfControl.Model;
using Microsoft.AspNetCore.RateLimiting;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapRateLimiting : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter("default", limiterOptions =>
            {
                limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
                limiterOptions.AutoReplenishment = true;
                limiterOptions.TokensPerPeriod = 5;
                limiterOptions.TokenLimit = 15;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return new RateLimitPartition<string>(context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? "default", ip =>
                {
                    return new TokenBucketRateLimiter()
                } );
            });
        });
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        //load ip rules from appsettings.json
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));


        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseIpRateLimiting();
        app.UseRateLimiter(new RateLimiterOptions
        {
            RejectionStatusCode = 429
        });
    }
}