#region

using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;

#endregion

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapController : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>());
        services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });
    }
}