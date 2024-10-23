using InversionOfControl.Model;
using MessagePack;
using MessagePack.Resolvers;

namespace AddictedProxy.Controllers.Hub.Bootstrap;

public class BootstrapHub : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR().AddJsonProtocol()
            .AddMessagePackProtocol(options =>
            {
                options.SerializerOptions = MessagePackSerializerOptions.Standard
                    .WithResolver(ContractlessStandardResolver.Instance)
                    .WithSecurity(MessagePackSecurity.UntrustedData);
            });
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<RefreshHub>("/refresh");
        }
    }
}