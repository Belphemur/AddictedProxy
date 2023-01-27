using InversionOfControl.Model;

namespace AddictedProxy.Controllers.Hub.Bootstrap;

public class BootstrapHub : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR().AddJsonProtocol();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<RefreshHub>("/refresh");
        }
    }
}