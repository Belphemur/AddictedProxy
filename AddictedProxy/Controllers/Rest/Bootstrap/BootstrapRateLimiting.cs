using AspNetCoreRateLimit;
using InversionOfControl.Model;

namespace AddictedProxy.Controllers.Rest.Bootstrap;

public class BootstrapRateLimiting : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        //load ip rules from appsettings.json
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

        // inject counter and rules stores
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseIpRateLimiting();
    }
}