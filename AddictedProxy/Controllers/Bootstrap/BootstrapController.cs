using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapController : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
                .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>());
        services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });

    }
}