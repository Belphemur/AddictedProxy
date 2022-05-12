#region

using AddictedProxy.Controllers.Rest.Serializer;
using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Controllers.Rest.Bootstrap;

public class BootstrapController : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.AddContext<SerializationContext>());
        services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllers();
        }
        
        app.UseHttpLogging();
        app.UseRouting();
        app.UseAuthorization();
    }
}