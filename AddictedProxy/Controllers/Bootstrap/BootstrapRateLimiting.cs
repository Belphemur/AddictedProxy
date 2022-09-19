using AspNetCoreRateLimit;
using InversionOfControl.Model;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapRateLimiting : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        //load ip rules from appsettings.json
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));


        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseIpRateLimiting();
    }
}