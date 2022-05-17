using AddictedProxy.Services.RateLimiting;
using InversionOfControl.Model;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Controllers.Hub.Bootstrap;

public class BootstrapHub : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR(options => options.AddFilter<SignalRRateLimiter>()).AddJsonProtocol();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<RefreshHub>("/refresh");
        }
    }
}