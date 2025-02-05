using InversionOfControl.Model;

namespace AddictedProxy.Services.Details.Bootstrap;

public class BootstrapShowDetails : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddScoped<IMediaDetailsService, MediaDetailsService>();
    }
}