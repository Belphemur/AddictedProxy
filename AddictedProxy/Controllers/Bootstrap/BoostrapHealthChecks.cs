using InversionOfControl.Model;

namespace AddictedProxy.Controllers.Bootstrap;

public class BoostrapHealthChecks : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
    }
}