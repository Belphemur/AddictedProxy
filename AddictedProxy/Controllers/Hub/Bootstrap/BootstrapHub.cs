using InversionOfControl.Model;
using MessagePack;
using MessagePack.Resolvers;

namespace AddictedProxy.Controllers.Hub.Bootstrap;

public class BootstrapHub : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR().AddJsonProtocol()
            .AddMessagePackProtocol();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<RefreshHub>("/refresh");
        }
    }
}